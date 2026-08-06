---
mode: agent
description: Phase A — rebase same-repo feature PRs onto their own base, produce a fresh per-PR patch for Phase B, and notify fork PR authors.
---

# Phase A — rebase feature PRs & produce per-PR patches (+ notify authors)

This is **Phase A** of the two-phase post-meeting workflow (see
`post-meeting-propagate.prompt.md` for **Phase B**, the serial per-version
sweep).

The complete chronology is:

1. **Phase A1 — this prompt's preparation pass.** Discover the actual draft
   and alpha branch names, dynamically enumerate the open feature PRs, report
   the resulting count, rebase or notify each PR as appropriate, and pin its
   patch inputs.
2. **Phase B — propagation and alpha reconstruction.** Run the serial
   per-version sweep. If Phase B moves a PR's draft base, it invokes only the
   rebase/patch machinery here to refresh that PR's pin.
3. **Phase A2 — this prompt's notification pass.** After Phase B reports its
   inventories, route future-version comments and send alpha-drift notices.

Do not run A2 before Phase B, and do not interpret A2 as input to the alpha
reconstruction. PR processing within A1 is independent and may run in
parallel after the enumeration snapshot is complete.

Phase A does two things:

1. **Rebase every open feature PR onto its own current base `draft-vN`**
   (update each PR per whether its head is in this repo or on a fork), and
2. **Produce a fresh `.patch` per PR** — this is the artifact **Phase B
   consumes** in its `alpha-vN` rebuild (Step D of the propagate prompt).
   Make the patch explicit: without it Phase B cannot surgically apply the
   fresh feature content onto the existing alpha structure.

> **Ordering note.** After Phase B propagates the committee changes into
> `draft-vN`, it does a *cheap re-rebase* of only the vN PRs whose base moved
> and **refreshes their patches** (propagate prompt, Step B.8). So the patches
> Phase B ultimately applies are the freshest post-propagation ones; the
> patches produced here are the starting point.

### Phase A2 notification and routing depend on Phase B output

Steps 4 and 5 below (alpha-drift detection and vNext-comment routing)
consume artifacts that **Phase B produces**:

- the **future-version comment inventory** (Step B.6 of the propagate prompt) — used by Step 5 below to route those notes to the appropriate feature PRs;
- the deletion inventory — used as context when judging rebase conflicts.

Run the discovery, rebase, pinning, and patch core (Steps 1–3) in A1. Run
Steps 4 and 5 only as A2, after Phase B has produced its report. Ask the
user for the propagate report (or its location) before running A2.

## Phase A1 — prepare pinned feature-PR inputs

### Step 1 — Discover branches and enumerate target PRs

Do not use a fixed branch list or PR count.

1. Fetch and list the actual remote branch names:

   ```bash
   git fetch upstream --prune
   git for-each-ref refs/remotes/upstream \
     --format='%(refname:strip=3)' \
     | grep -E '^(draft-v[0-9]+|alpha-v[0-9]+|v[0-9]+-alpha)$' \
     | sort -V
   ```

2. Starting at the user-supplied draft (default `draft-v8`), discover later
   `draft-vN` branches and the corresponding alpha branch named either
   `alpha-vN` or `vN-alpha`. Use the name that actually exists;
   `v11-alpha` is valid. Stop if both alpha spellings exist for a version or
   the topology is ambiguous. Confirm the result against
   `admin/branch-diagram.md`.
3. Feature PRs target draft branches. For every discovered target
   `draft-vN`, list all open PRs with an explicit high limit and capture
   `number`, `baseRefName`, `headRefOid`, `headRefName`,
   `headRepositoryOwner`, `isDraft`, `author`, `title`, and `url`:

   ```bash
   gh pr list --base <base> --state open --limit 1000 \
     --json number,baseRefName,headRefOid,headRefName,headRepositoryOwner,isDraft,author,title,url
   ```

4. Combine and de-duplicate the results by PR number. Report the exact
   discovered branch chain and resulting feature-PR count before processing
   any PR. The count is run data, never prompt text.

### Step 2 — Create the pinned run manifest

Create a persistent, untracked run directory under `.git`:

```bash
RUN_ID="$(date -u +%Y%m%dT%H%M%SZ)"
RUN_DIR="$(git rev-parse --git-path "post-meeting/$RUN_ID")"
mkdir -p "$RUN_DIR/patches"
```

Create `$RUN_DIR/feature-prs.json` from the dynamic enumeration. Each record
must contain at least:

- PR number and base branch;
- head branch and repository owner;
- enumerated head SHA;
- patch path and processing status;
- `capturedAt` and `cutoffTime` in UTC.

Before Phase B starts, recheck every head and set each record's `cutoffTime`
to the time of that successful check. Finalize the manifest with a top-level
UTC `finalizedAt`, the total PR count, and the exact discovered branch
chain. Phase B must reject a missing, duplicate, or count-mismatched
manifest.

### Step 3 — Process each enumerated PR

For each manifest PR:

1. Query `gh pr view <num> --json headRefOid,baseRefName,state` immediately
   before processing. If the PR closed, changed base, or its head differs
   from the enumerated SHA, update the enumeration and restart that PR's
   preparation; do not continue from a stale snapshot.
2. Process the PR according to its head repository:

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
  7. Immediately before push, query `headRefOid` again. If it differs from
     the remote SHA captured immediately before checkout, stop and restart
     this PR; the author moved it during the run. Otherwise:
     `git push --force-with-lease upstream <headRefName>`. Record success
     with the new SHA.
  8. **Produce the per-PR patch (Phase B input).** Capture the PR's commits
     relative to its base as a patch file so Phase B can surgically apply them
     onto `alpha-vN`:

     ```bash
     mkdir -p "$RUN_DIR/patches/<base>"
     git format-patch "upstream/<base>..<headRefName>" \
       --stdout > "$RUN_DIR/patches/<base>/pr-<num>.patch"
     ```

     Record the patch path, new head SHA, and UTC `capturedAt` in the
     manifest. This is the fresh vN patch Phase B consumes; if Phase B later
     moves `<base>` via committee propagation, its cheap re-rebase updates
     the SHA, patch, and cutoff (propagate prompt, Step B.8).

- If the head is on a fork, **do not attempt to rebase.** Post the
  **please-rebase** comment below. Avoid duplicate comments: skip if any existing comment on the PR already contains the marker `<!-- post-meeting-rebase-notice -->`. Then capture a **best-effort patch** from the PR's current head so Phase B still has an input for the `alpha-vN` rebuild — note it may need refreshing once the author rebases:

  ```bash
  mkdir -p "$RUN_DIR/patches/<base>"
  gh pr diff <num> --patch > "$RUN_DIR/patches/<base>/pr-<num>.patch"
  ```

  Record the current `headRefOid`, patch path, UTC `capturedAt`, and
  `bestEffort: true` in the manifest. Phase B must recheck the fork PR head
  and stop if it moved after cutoff.

After all PRs are processed, query every manifest PR's `headRefOid` one more
time. If any head moved during the long run, refresh it. Otherwise set that
record's `cutoffTime`, then set the top-level `finalizedAt`. Report the final
PR count and cutoff range.

## Phase A2 — notify and route after Phase B

### Step 4 — Detect feature-PR drift across cycles

A feature PR open against `draft-vN` may have been edited between meetings,
while an *earlier* version of the same feature was already merged into that
version's discovered alpha branch at a previous meeting. Plain propagation
will not carry the new edits onto the alpha until the next meeting. Surface
these so authors can decide whether to open a separate alpha-targeted PR
sooner.

For every open PR P with base `draft-vN`:

1. Read its files: `gh pr view <num> --json files,author,number,headRefOid`.
2. Look for prior commits on the discovered alpha branch by the same author
   touching the same files:

   ```bash
   git log upstream/<alpha-for-vN> --author='<login>' -- <files…>
   ```

3. If prior commits exist and the current PR head SHA introduces changes to
   those files not yet on the discovered alpha, record P as **"alpha has
   stale version"**.

Surface the list to the user. **Do not** cherry-pick or push to the alpha.
Instead, post the **alpha-drift notice** comment template (below) on each
affected PR; the marker dedupes on rerun.

### Step 5 — Route future-version comments to feature PRs

Consume the future-version comment inventory from the propagate prompt's
report. For each entry (file:line, comment text, source branch, best-guess
target `vN`):

1. List candidate open PRs:

   ```bash
   gh pr list --base draft-vN --state open --json number,title,files,url
   gh pr list --base <alpha-for-vN> --state open --json number,title,files,url
   ```

2. Match the comment to candidate PRs by:
   - **file path**: the comment's file appears in the PR's changed files;
   - **proximity**: the comment's line is within or near a hunk modified by
     the PR (best-effort; surface multiple candidates rather than guessing).
3. For each (comment, candidate-PR) pair, post the **vNext-routing** comment
   template (below). The HTML marker
   `<!-- post-meeting-vnext-routing -->` includes the source `file:line` so
   re-runs do not duplicate it. Skip the comment if that marker and the same
   `file:line` already exist on the PR.
4. List comments with **no** candidate PR in the final report under
   "unrouted vNext comments" for user triage.

If `vN` is missing or unparseable from the comment, list the entry as
unrouted and surface it in the report.

**This step never edits `standard/*.md`.** The prompts neither apply nor
remove these comments; the matched PR's author decides whether to fold the
change in or open a new PR.

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
- The dynamically discovered branch chain, exact feature-PR count, manifest
  path, per-PR UTC cutoffs, and manifest finalization time
- **Per-PR patches produced** (path `$RUN_DIR/patches/<base>/pr-<num>.patch`
  per PR, with pinned head SHA) — the Phase B input set the alpha rebuild
  consumes; flag any fork PRs whose patch is best-effort/stale pending author
  rebase
- Any PR head that moved during the run and how its manifest entry was
  refreshed
- PRs left for manual rebase (with reason — including any failing `TOC002` references from the post-rebase cross-reference check)
- Fork PRs commented on
- PRs that already had the notice (skipped)
- PRs flagged with the **alpha-drift notice** (Step 4)
- vNext comments routed (PR number + source `file:line`) and any **unrouted vNext comments** for human triage (Step 5)

## Safety rules

- Never close or merge a feature PR.
- Never push to a fork.
- Always use `--force-with-lease`, never `--force`.
- If `gh pr checkout` fails (e.g. permissions), record and skip.
