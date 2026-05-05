---
mode: agent
description: Propagate post-meeting changes through all draft and alpha branches.
---

# Post-meeting: propagate changes through draft & alpha branches

You are propagating the changes merged into the current "starting" draft branch
(after a TC49-TG2 committee meeting) forward through every future draft and
alpha branch. See `admin/branch-diagram.md` for the rationale.

## Assumptions

- The committee secretary has already merged all approved PRs into the
  starting branch (default: `draft-v8`). Do **not** merge any other open PRs.
- The `.github/workflows/update-on-merge.yaml` workflow will open an automated
  PR titled "Automated Section renumber and grammar extraction" on each branch
  that receives a push. These auto-PRs **must be merged** before the next
  propagation hop.
- You have `git` and `gh` available, and push permission to this repo.

## Propagation chain

Walk these branches in order. Each step merges the previous branch into the
next with a regular merge commit and pushes.

1. `draft-v8`   (starting branch — no merge in; just merge its auto-PR)
2. `draft-v9`   ← merges `draft-v8`
3. `v9-alpha`   ← merges `draft-v9`
4. `draft-v10`  ← merges `v9-alpha`
5. `v10-alpha`  ← merges `draft-v10`
6. `draft-v11`  ← merges `v10-alpha`
7. `v11-alpha`  ← merges `draft-v11`
8. `draft-v12`  ← merges `v11-alpha`

> When new version branches are added (v12-alpha, draft-v13, etc.), append
> them here in the same pattern: each `draft-v(N+1)` merges `vN-alpha`, and
> each `vN-alpha` merges `draft-vN`. Also add the new branches to
> `.github/workflows/update-on-merge.yaml` `on.push.branches`.

If the user names a different starting branch, start there and propagate to
every later branch in the list.

## Procedure

Before starting, confirm the chain with the user and show which branches will
be touched. Then for each branch in order:

### Step A — Process auto-PR on the current branch

1. `git fetch --all --prune`
2. `git checkout <branch>` and `git pull --ff-only`
3. Find the auto-PR opened by `update-on-merge.yaml`:

   ```bash
   gh pr list --base <branch> --state open \
     --search 'in:title "Automated Section renumber and grammar extraction"' \
     --json number,headRefName,mergeable,mergeStateStatus
   ```

4. If found:
   - Wait until checks pass (`gh pr checks <num> --watch` is acceptable, with
     a reasonable timeout — if it doesn't go green within a few minutes, stop
     and ask the user).
   - Merge it with a **merge commit**:
     `gh pr merge <num> --merge --delete-branch`.
   - `git pull --ff-only` on the branch.
5. If not found and this is the starting branch, ask the user whether to wait,
   run the tools locally (`tools/run-smarten.sh`,
   `tools/run-section-renumber.sh`), or proceed without them. Do not run
   tools locally without explicit consent.
6. If not found on a downstream branch (it should appear after the merge in
   step B below), continue.

### Step B — Merge from previous branch (skip on the starting branch)

1. Still on `<branch>`, run:

   ```bash
   git merge --no-ff origin/<previous-branch> \
     -m "Post-meeting propagation: merge <previous-branch> into <branch>"
   ```

2. **Conflicts:**
   - If the conflicts are limited to `standard/grammar.md`, the TOC/anchor
     output of the renumber tool, or other purely mechanical regenerated
     files where "take the version from the previous (upstream) branch" is
     correct, resolve by taking the upstream side, stage, and commit.
   - For any conflict in prose/normative text, in `tools/`, in `.github/`, or
     anything you are not 100% sure is mechanical: **abort the merge**
     (`git merge --abort`), report the conflicting files and a brief diff
     summary to the user, and stop. Do not push.
3. Push: `git push origin <branch>`.
4. The push triggers `update-on-merge.yaml`, which will open a fresh auto-PR.
   Loop back to Step A for this branch before moving on.

### Step C — Move to the next branch

Repeat A–B for the next branch in the chain. Continue until the chain is
done.

## Reporting

When finished (or when stopped on a conflict), produce a short report:

- For each branch: action taken (auto-PR merged, propagation merge SHA,
  skipped).
- Any branches where you stopped and why.
- A reminder to the user to run `post-meeting-rebase-prs.prompt.md` next.

## Safety rules

- Never force-push to any `draft-v*` or `v*-alpha` branch.
- Never use `git reset --hard` on a tracked branch.
- Never merge any PR other than the automated renumber/grammar PR.
- If `gh` shows the auto-PR's checks failing, stop and ask the user.
