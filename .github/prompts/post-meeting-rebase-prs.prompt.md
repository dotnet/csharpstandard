---
mode: agent
description: Rebase same-repo feature PRs and notify fork PR authors after propagation.
---

# Post-meeting: rebase / notify feature PRs

After `post-meeting-propagate.prompt.md` has updated all draft/alpha branches, every open feature PR is now stale relative to its base. Update each PR according to whether its head branch is in this repo or on a fork.

This prompt also consumes two artifacts from the propagate run:

- the **future-version comment inventory** (Step B.6 of the propagate prompt) — used by Step 0.5 below to route those notes to the appropriate feature PRs;
- the deletion inventory — used as context when judging rebase conflicts.

Ask the user for the propagate report (or its location) before starting.

## Procedure

### Step 0 — Detect feature-PR drift across cycles

A feature PR open against `draft-vN` may have been edited between meetings, while an *earlier* version of the same feature was already merged into `vN-alpha` at a previous meeting. Plain propagation will not carry the new edits onto `vN-alpha` until the next meeting. Surface these so authors can decide whether to open a separate alpha-targeted PR sooner.

For every open PR P with base `draft-vN`:

1. Read its files: `gh pr view <num> --json files,author,number,headRefOid`.
2. Look for prior commits on `vN-alpha` by the same author touching the same files:

   ```bash
   git log upstream/vN-alpha --author='<login>' -- <files…>
   ```

3. If prior commits exist and the current PR head SHA introduces changes to those files not yet on `vN-alpha`, record P as **"alpha has stale version"**.

Surface the list to the user. **Do not** cherry-pick or push to `vN-alpha`. Instead, post the **alpha-drift notice** comment template (below) on each affected PR; the marker dedupes on rerun.

### Step 0.5 — Route future-version comments to feature PRs

Consume the future-version comment inventory from the propagate prompt's report. For each entry (file:line, comment text, source branch, best-guess target `vN`):

1. List candidate open PRs:

   ```bash
   gh pr list --base draft-vN --state open --json number,title,files,url
   gh pr list --base vN-alpha --state open --json number,title,files,url
   ```

2. Match the comment to candidate PRs by:
   - **file path**: the comment's file appears in the PR's changed files;
   - **proximity**: the comment's line is within or near a hunk modified by the PR (best-effort; surface multiple candidates rather than guessing).

3. For each (comment, candidate-PR) pair, post the **vNext-routing** comment template (below) on the PR. The HTML marker `<!-- post-meeting-vnext-routing -->` includes the source `file:line` so re-runs do not duplicate (skip if a comment with that marker and the same `file:line` already exists on the PR).

4. Comments with **no** candidate PR are listed in the final report under "unrouted vNext comments" for the user to triage.

If `vN` is missing or unparseable from the comment, list the entry as unrouted and surface it in the report.

**This step never edits `standard/*.md`.** The prompts neither apply nor remove these comments; the matched PR's author decides whether to fold the change in or open a new PR.

### Step 1 — List target base branches

List target base branches (the propagation chain — see the propagate prompt). Default: `draft-v8 draft-v9 alpha-v9 draft-v10 alpha-v10 draft-v11 v11-alpha draft-v12`. Allow the user to override.

> **Branch-naming is inconsistent across versions** (e.g. `alpha-v9` vs
> `v11-alpha`). Always verify actual branch
> names with `git branch -r | grep upstream/` before starting.

### Step 2 — For each base branch, list and process open PRs

For each base branch, list open PRs:

```bash
gh pr list --base <base> --state open \
  --json number,title,headRefName,headRepositoryOwner,isDraft,author,url
```

For each PR returned:

- If `headRepositoryOwner.login` matches this repo's owner (`dotnet`), the head is in this repo. **Rebase it:**
  1. `gh pr checkout <num>` — if this fails to fast-forward (branch has
     diverged from upstream), run `git reset --hard upstream/<headRefName>`
     to sync to the upstream version of the PR branch before proceeding.
  2. **Capture the pre-rebase SHA** so we can recover if the post-rebase cross-reference check fails: `PRE=$(git rev-parse HEAD)`.
  3. `git fetch upstream <base>`
  4. `git rebase upstream/<base>` — **use `upstream/`** (the main repo
     remote), not `origin/`. The PR branches track `upstream`.
  5. **If the rebase has conflicts:**
     - **Binary conflicts** in `.github/workflows/dependencies/` (e.g.
       `GrammarTestingEnv.tgz`): resolve by taking the PR's version
       (`git checkout --theirs <file> && git add <file>`) and continue
       the rebase. These are feature-specific test artifacts and the PR's
       version is correct.
     - **Formatting-only conflicts** in `standard/*.md` (e.g. table
       pipe characters, smart-quote vs straight-quote differences):
       take the PR's version (`git checkout --theirs <file> && git add
       <file>`) and continue. The PR's formatting fix is the intended
       change.
     - **Independent additive conflicts** in `standard/*.md` (two
       features each added text to the same list or paragraph, but the
       additions are logically independent): combine both additions in
       the correct order, resolve, and continue.
     - **Structural/semantic conflicts** in `standard/*.md` (e.g.
       grammar reorganization where the base refactored a production
       hierarchy and the PR uses the old structure), or conflicts in
       `tools/`: `git rebase --abort`, record the PR as "needs manual
       rebase", and move on. Do not attempt to resolve conflicts that
       require the author's design decision.
  6. **Cross-reference check before push.** Run the renumber tool in dry-run mode against the rebased worktree:

     ```bash
     ( cd tools && dotnet run --project StandardAnchorTags -- \
         --owner dotnet --repo csharpstandard --dryrun )
     ```

     If any *new* `TOC002` diagnostic fires that was not already present
     on the PR branch before rebase, the rebase has introduced broken
     cross-references. Recover with `git reset --hard "$PRE"`, record
     the PR as "needs manual rebase", and move on. To distinguish new
     from pre-existing: check the same ref against
     `upstream/<headRefName>` (the pre-rebase state). Pre-existing
     TOC002s from incomplete feature work are expected and do not block
     the push.
  7. Otherwise: `git push --force-with-lease upstream <headRefName>`.
     Record success with the new SHA.

- If the head is on a fork, **do not attempt to rebase.** Post the
  **please-rebase** comment below. Avoid duplicate comments: skip if any existing comment on the PR already contains the marker `<!-- post-meeting-rebase-notice -->`.

## Comment templates

The HTML comment is a marker so re-runs don't duplicate.

**Please-rebase (fork PRs and any PR not yet notified):**

```markdown
<!-- post-meeting-rebase-notice -->
The base branch `{{base}}` has been updated following the latest TC49-TG2 committee meeting. Please rebase this PR onto the latest `{{base}}` and resolve any conflicts. Thanks!
```

**Attempted-rebase (same-repo PRs that hit conflicts):**

```markdown
<!-- post-meeting-rebase-notice -->
I attempted to rebase this PR onto the updated `{{base}}` after the latest TC49-TG2 committee meeting but encountered conflicts. Please rebase locally and resolve them. Thanks!
```

**Alpha-drift notice (feature PRs whose alpha branch already has an older version of this feature):**

```markdown
<!-- post-meeting-rebase-notice -->
An earlier version of this feature is already present on `{{alpha}}` from a prior meeting. Edits made to this PR since then are **not** yet on `{{alpha}}`; they will land at the next propagation. If you need them on `{{alpha}}` sooner, please open a separate PR targeting `{{alpha}}`.
Thanks!
```

**vNext-routing (notes left on an older version that target this PR's
version):**

```markdown
<!-- post-meeting-vnext-routing source={{file}}:{{line}} -->
While propagating post-meeting changes, the following note targeting `{{target_version}}` was found on `{{source_branch}}` at `{{file}}:{{line}}`:

> {{comment_text}}

This PR appears to touch the same area. Please consider whether this note applies — either fold the change into this PR or open a follow-up. The note in `standard/*.md` was **not** edited or removed by tooling.
```

Post via `gh pr comment <num> --body-file <tempfile>`.

## Reporting

At the end, output a table grouped by base branch:

- PRs successfully rebased + force-pushed (with new SHA)
- PRs left for manual rebase (with reason — including any failing `TOC002` references from the post-rebase cross-reference check)
- Fork PRs commented on
- PRs that already had the notice (skipped)
- PRs flagged with the **alpha-drift notice** (Step 0)
- vNext comments routed (PR number + source `file:line`) and any **unrouted vNext comments** for human triage (Step 0.5)

## Safety rules

- Never close or merge a feature PR.
- Never push to a fork.
- Always use `--force-with-lease`, never `--force`.
- If `gh pr checkout` fails (e.g. permissions), record and skip.
