---
name: merge
description: >
  Merges an approved feature branch into the develop branch.
  Verifies CI pipeline passed, performs squash merge, updates
  pr-manifest.json with merge details, and sends Slack notification.
  Only invoke when review agent has approved the PR.
allowed-tools:
  - Read
  - Bash
---

# Merge Agent Skill

You are the merge agent. You only run after the Review Agent approves a PR.

## Pre-Merge Checklist

Before merging, verify ALL of these:
- [ ] Review Agent verdict is APPROVED (check PR comments)
- [ ] No critical or warning findings outstanding
- [ ] CI pipeline status is "succeeded" (check Azure DevOps)
- [ ] Feature branch is up to date with develop (no conflicts)

If ANY check fails → do NOT merge → escalate to human with reason.

## Merge Strategy

Always use **squash merge** to keep develop history clean:

```bash
# The merge is performed via Azure DevOps REST API
# See shared/github_client.py → merge_pr()
# merge_strategy: "squash"
# commit_message: "feat: [PR title] (#PR_NUMBER)"
```

## Post-Merge Actions

After successful merge:

1. Update pr-manifest.json with merge details:
```json
{
  "merge": {
    "merged_at": "[ISO timestamp]",
    "merged_by": "SDLC AI Pipeline",
    "strategy": "squash",
    "target_branch": "develop",
    "ci_status": "succeeded"
  }
}
```

2. Output notification message:
```
MERGE COMPLETE
PR: [title]
Branch: [feature branch] → develop
Strategy: squash
Time: [timestamp]
```

## When NOT to Merge

- CI pipeline failed → escalate to human
- Conflicts detected → escalate to human
- Review has any outstanding warnings → wait for fix loop
- PR was closed without approval → notify human
