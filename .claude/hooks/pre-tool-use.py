"""
SDLC AI — Pre-Tool-Use Safety Hook
Runs before every tool call in the agent loop.
Blocks dangerous operations and enforces guardrails.
"""

import json
import sys
import os

def check(input_data: dict) -> dict:
    tool_name = input_data.get("tool_name", "")
    tool_input = input_data.get("tool_input", {})

    # ── Block dangerous bash commands ──────────────────────────────────────
    if tool_name == "Bash":
        command = tool_input.get("command", "")

        # Never delete main/develop/master branches
        blocked_git = [
            "git push --force",
            "git push -f",
            "git branch -D main",
            "git branch -D develop",
            "git branch -D master",
            "git reset --hard HEAD~",
            "git clean -fd",
        ]
        for blocked in blocked_git:
            if blocked in command:
                return {
                    "hookSpecificOutput": {
                        "hookEventName": "PreToolUse",
                        "permissionDecision": "deny",
                        "permissionDecisionReason": (
                            f"BLOCKED by safety hook: '{blocked}' is not allowed. "
                            "This operation could destroy work."
                        ),
                    }
                }

        # Never drop databases
        blocked_db = ["DROP DATABASE", "DROP TABLE", "TRUNCATE TABLE", "DELETE FROM"]
        cmd_upper = command.upper()
        for blocked in blocked_db:
            if blocked in cmd_upper:
                return {
                    "hookSpecificOutput": {
                        "hookEventName": "PreToolUse",
                        "permissionDecision": "deny",
                        "permissionDecisionReason": (
                            f"BLOCKED: Destructive database operation '{blocked}' "
                            "requires human approval."
                        ),
                    }
                }

        # Never install packages without recording (warn only — don't block)
        if "pip install" in command or "npm install" in command or "dotnet add package" in command:
            print(f"[HOOK WARNING] Package installation detected: {command}", file=sys.stderr)

    # ── Block writes to protected files ────────────────────────────────────
    if tool_name in ("Write", "Edit"):
        file_path = tool_input.get("file_path", tool_input.get("path", ""))

        protected = [
            ".env",
            "shared/llm.py",
            ".claude/hooks/pre-tool-use.py",
            "orchestrator/state.py",
        ]
        for p in protected:
            if file_path.endswith(p) or file_path == p:
                return {
                    "hookSpecificOutput": {
                        "hookEventName": "PreToolUse",
                        "permissionDecision": "deny",
                        "permissionDecisionReason": (
                            f"BLOCKED: '{p}' is in the Do Not Touch list from CLAUDE.md. "
                            "Human approval required to modify this file."
                        ),
                    }
                }

    # ── Allow all other operations ──────────────────────────────────────────
    return {}


if __name__ == "__main__":
    try:
        input_data = json.loads(sys.stdin.read())
        result = check(input_data)
        print(json.dumps(result))
    except Exception as e:
        # Never crash — just allow on hook error
        print(json.dumps({}))
