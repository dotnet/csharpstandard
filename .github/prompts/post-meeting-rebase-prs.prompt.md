---
mode: agent
description: Rebase same-repo feature PRs and notify fork PR authors after propagation.
---

# Post-meeting: rebase / notify feature PRs

After `post-meeting-propagate.prompt.md` has updated all draft/alpha branches,
every open feature PR is now stale relative to its base. Update each PR
according to whether its head branch is in this repo or on a fork.

## Procedure

1. List target base branches (the propagation chain — see the propagate
   prompt). Default:
   `draft-v8 draft-v9 v9-alpha draft-v10 v10-alpha draft-v11 v11-alpha draft-v12`.
   Allow the user to override.

2. For each base branch, list open PRs:

   ```bash
   gh pr list --base <base> --state open \
     --json number,title,headRefName,headRepositoryOwner,isDraft,author,url
   ```

3. For each PR returned:

   - If `headRepositoryOwner.login` matches this repo's owner (`dotnet`), the
     head is in this repo. **Rebase it:**
     1. `gh pr checkout <num>`
     2. `git fetch origin <base>`
     3. `git rebase origin/<base>`
     4. **If the rebase has conflicts:** `git rebase --abort`, record the PR
        as "needs manual rebase", post the "attempted rebase" comment below,
        and move on. Do not attempt to resolve feature-text conflicts.
     5. Otherwise: `git push --force-with-lease`. Record success.

   - If the head is on a fork, **do not attempt to rebase.** Post the
     "please rebase" comment below. Avoid duplicate comments: skip if any
     existing comment on the PR already contains the marker
     `<!-- post-meeting-rebase-notice -->`.

4. Skip draft PRs unless the user asks otherwise.

## Comment templates

The HTML comment is a marker so re-runs don't duplicate.

**Please-rebase (fork PRs and any PR not yet notified):**

```markdown
<!-- post-meeting-rebase-notice -->
The base branch `{{base}}` has been updated following the latest TC49-TG2
committee meeting. Please rebase this PR onto the latest `{{base}}` and
resolve any conflicts. Thanks!
```

**Attempted-rebase (same-repo PRs that hit conflicts):**

```markdown
<!-- post-meeting-rebase-notice -->
I attempted to rebase this PR onto the updated `{{base}}` after the latest
TC49-TG2 committee meeting but encountered conflicts. Please rebase locally
and resolve them. Thanks!
```

Post via `gh pr comment <num> --body-file <tempfile>`.

## Reporting

At the end, output a table grouped by base branch:

- PRs successfully rebased + force-pushed (with new SHA)
- PRs left for manual rebase (with reason)
- Fork PRs commented on
- PRs that already had the notice (skipped)

## Safety rules

- Never close or merge a feature PR.
- Never push to a fork.
- Always use `--force-with-lease`, never `--force`.
- If `gh pr checkout` fails (e.g. permissions), record and skip.
