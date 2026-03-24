# CLAUDE.md — SDLC AI Pipeline

## Project Overview
This is the **SDLC AI Pipeline** — an autonomous software delivery system.
It takes a user story and repository URL, and autonomously plans, codes,
reviews, and merges software changes using AI agents.

**Owner:** S&P Global Platform Engineering
**Stack:** Python, FastAPI, Azure OpenAI (gpt-5.2)
**Purpose:** Autonomous SDLC from user story to merged PR

---

## Architecture

```
User Story + Repo URL
        ↓
  [Intake Agent]      → Asks gap questions, generates proj.md + pr-manifest.json
        ↓
  [Coding Agent]      → Clones repo, writes code, runs build/tests, commits, raises PR
        ↓
  [Review Agent]      → Quality + Security parallel review, posts findings to PR
        ↓  (if approved)
  [Merge Agent]       → Auto-merges to develop, notifies human
```

All agents are orchestrated by `orchestrator/pipeline.py` via a state machine.
One FastAPI app (`main.py`) exposes all routes. One UI (`frontend/index.html`).

---

## Agent Responsibilities

| Agent | Skill | Purpose |
|---|---|---|
| Intake | `.claude/skills/intake/SKILL.md` | Gap detection, clarification, file generation |
| Coding | `.claude/skills/coding/SKILL.md` | Autonomous code writing, build, test, commit |
| Review | `.claude/skills/review/SKILL.md` | Quality + security review, PR feedback |
| Merge  | `.claude/skills/merge/SKILL.md`  | Auto-merge, notification |

---

## Key Files

```
main.py                          ← Single FastAPI entry point, all routes
orchestrator/pipeline.py         ← State machine, agent coordination
orchestrator/state.py            ← PipelineRun model, status enums
agents/intake_agent.py           ← Intake agent logic
agents/coding_agent.py           ← Agentic coding loop (Claude Agent SDK)
agents/review_agent.py           ← Parallel quality + security review
agents/merge_agent.py            ← PR merge + notification
shared/llm.py                    ← Single Azure OpenAI client
shared/git_client.py             ← All git operations
shared/github_client.py        ← Azure DevOps REST API
frontend/index.html              ← Single-page unified UI
.claude/skills/*/SKILL.md        ← Agent skill definitions
.claude/hooks/                   ← Safety guardrails
docs/decisions/                  ← Architecture decision records
docs/runbooks/                   ← Operational runbooks
```

---

## Coding Conventions

- All LLM calls go through `shared/llm.py` — never call Azure OpenAI directly
- All git operations go through `shared/git_client.py`
- All GitHub operations go through `shared/github_client.py`
- Agent state always stored in `orchestrator/pipeline.py` sessions dict
- Never hardcode credentials — always use `os.getenv()`
- Use `httpx.Client(verify=False)` for all HTTP (corporate SSL)
- All async functions use `async/await` — never `.result()` or `.wait()`
- Log every significant action with `print(f"[AGENT] {message}")`

---

## Agent Instructions

When writing or modifying code in this project:

1. **Read this file first** — understand the architecture before touching anything
2. **One responsibility per agent** — don't add coding logic to the intake agent
3. **State machine is sacred** — all pipeline transitions go through `pipeline.py`
4. **Skills drive behavior** — agent prompts live in SKILL.md files, not in Python
5. **Never block the event loop** — all I/O must be async
6. **Hooks are guardrails** — never bypass `.claude/hooks/` safety checks
7. **The shared/ layer is the only way** — agents never call Azure/Git directly

---

## Do Not Touch

- `shared/llm.py` — LLM client is configured for corporate SSL, do not modify
- `.env` — credentials file, never commit, never log values
- `orchestrator/state.py` — state enum changes break existing sessions
- `.claude/hooks/pre-tool-use.py` — safety guardrail, never disable

---

## Environment Variables Required

```
AZURE_OPENAI_ENDPOINT       Azure OpenAI / APIM endpoint URL
AZURE_OPENAI_KEY            API key
AZURE_OPENAI_DEPLOYMENT     Model deployment name (e.g. gpt-5.2)
AZURE_OPENAI_API_VERSION    API version (e.g. 2025-01-01-preview)
GIT_TOKEN                   Azure DevOps PAT or GitHub token

GIT_USER_NAME               Commit author name
GIT_USER_EMAIL              Commit author email
ANTHROPIC_API_KEY           For Claude Agent SDK (coding agent)
```

---

## Pattern References

When writing new agents, follow these existing patterns:
- LLM call pattern: `shared/llm.py → call_llm(messages)`
- Git pattern: `shared/git_client.py → GitClient`
- Agent pattern: `agents/intake_agent.py`
- State transition: `orchestrator/pipeline.py → transition()`
