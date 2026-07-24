---
mode: agent
description: Phase B — serial per-version sweep that propagates committee changes and finalizes each alpha (committee changes + fresh patches) before advancing.
---

# Post-meeting: propagate changes through draft & alpha branches (Phase B)

You are propagating the changes merged into the current "starting" draft branch (after a TC49-TG2 committee meeting) forward through every future draft and alpha branch. See `admin/branch-diagram.md` for the rationale.

## Phase B of the two-phase post-meeting workflow

The monthly post-meeting work is split into two phases (see
`post-meeting-rebase-prs.prompt.md` for **Phase A**):

- **Phase A — parallel PR rebase (runs first / in parallel).** Rebase all
  ~54 open feature PRs onto their own base `draft-vN`. Independent per
  PR/version → embarrassingly parallel. Produces a **fresh `.patch` per PR**
  that this phase consumes.
- **Phase B — this prompt: a single serial sweep up the chain, atomic per
  version.** For each version N in chain order, (1) propagate the
  committee-merged changes into `draft-vN`, (2) real-renumber `draft-vN`
  (watch the concept-drift trap), (3) cheap re-rebase only the vN PRs whose
  base moved in step 1, (4) **rebuild `alpha-vN`** by surgically applying the
  fresh vN patches onto the EXISTING alpha structure (Option 1 — never a
  from-scratch structural reorg), dry-run-validate, then (5) advance so
  `draft-v(N+1)` is based on the now-finalized `alpha-vN`.

### ⚠️ Ordering invariant (the crux — do not violate)

**Never propagate or build anything downstream of `alpha-vN` until
`alpha-vN` is finalized with BOTH the committee-merged changes AND the fresh
feature-PR patches.** In one sentence: `alpha-vN` = finalized `draft-vN` +
fresh vN patches, constructed in **ONE pass**, before `draft-v(N+1)` is built
on it.

This is what eliminates the second (double) propagation cascade hit in the
2026-07 cycle: propagating across the whole chain first builds
`draft-v10..v12` on alphas whose feature-PR content is still **stale**, so
refreshing the patches later (e.g. the #1458 records rebase) invalidates
everything downstream and forces a second full cascade. Finalizing each alpha
before building on it removes that second pass.

## Assumptions

- The committee secretary has already merged all approved PRs into the starting branch (default: `draft-v8`). Do **not** merge any other open PRs.
- The `.github/workflows/update-on-merge.yaml` workflow will open an automated PR titled "Automated Section renumber and grammar extraction" on each branch that receives a push. These auto-PRs **must be merged** before the next propagation hop.
- Auto-PRs may show a `BLOCKED` merge state due to branch protection rules requiring review approval — this is normal and not a CI failure. Wait for checks to pass, then merge.
- You have `git` and `gh` available, and push permission to this repo.

## Propagation chain

Walk these branches in order. Each step merges the previous branch into the next with a regular merge commit and pushes.

1. `draft-v8`   (starting branch — no merge in; just merge its auto-PR)
2. `draft-v9`   ← merges `draft-v8`
3. `alpha-v9`   ← merges `draft-v9`
4. `draft-v10`  ← merges `alpha-v9`
5. `alpha-v10`  ← merges `draft-v10`
6. `draft-v11`  ← merges `alpha-v10`
7. `v11-alpha`  ← merges `draft-v11`
8. `draft-v12`  ← merges `v11-alpha`

> **Walk this chain as a per-version, atomic-per-version sweep.** For each
> version N, finish `draft-vN` (propagate committee changes + renumber +
> cheap re-rebase of moved vN PRs) **and** finalize `alpha-vN` (rebuild it
> from the fresh vN patches — Step D) BEFORE advancing to `draft-v(N+1)`. The
> alpha branch (`alpha-vN` / `vN-alpha`) is never just a plain merge of its
> draft: it must also receive the fresh feature-PR patches in the same pass,
> per the ordering invariant above.

> **Branch-naming is inconsistent across versions** (e.g. `alpha-v9` vs
> `v11-alpha`). Always verify actual branch
> names with `git branch -r | grep upstream/` before starting.
>
> When new version branches are added, append them here using the actual
> branch names on the remote. Also add any new branches to
> `.github/workflows/update-on-merge.yaml` `on.push.branches`.
>
> Note: `update-on-merge.yaml` currently only triggers on `standard-v6`,
> `standard-v7`, `draft-v8`, `draft-v9`, `draft-v11`, and `draft-v12`.
> Alpha branches are **not** covered, so no auto-PR will appear on those
> branches — the renumber/grammar tools must be run by a later merge into
> the next draft branch.

If the user names a different starting branch, start there and propagate to
every later branch in the list.

## Pre-flight (run once, before walking the chain)

These artifacts are referenced by Step B (conflict resolution) and Step B.6
(future-version comment inventory). Capture them up front and persist them
for the duration of the run.

1. **Baseline.** Determine the SHA on each branch in the chain at the
   *previous* propagation. A reliable proxy is the most recent merge commit whose subject begins with `Post-meeting propagation:` on that branch (`git log --grep '^Post-meeting propagation:' -1 --format=%H origin/<branch>`). Record `BASE_<branch>` for each branch. If a branch has no such commit yet, fall back to the branch point and warn the user.

2. **Deletion inventory** for the starting branch. Identify text the
   committee removed in this cycle so Step B can recognize resurrection:

   ```bash
   # Files where lines were removed since the previous propagation:
   git log "$BASE_<starting>"..origin/<starting> --diff-filter=MD \
     --name-only --pretty=format: -- standard/ | sort -u

   # Hunks (negative lines) for review:
   git log "$BASE_<starting>"..origin/<starting> -p -- standard/ \
     > /tmp/starting-deletions.patch
   ```

   Show the user the list of PRs merged this cycle (`gh pr list --base <starting> --state merged --search 'merged:>=<last-meeting-date>'`) and ask which performed substantive prose removals. Persist that list.

3. **vNext-comment baseline** for every branch in the chain. Snapshot the current set of HTML comments in `standard/*.md` mentioning a future version, so Step B.6 can diff against it after each merge:

   ```bash
   for b in <chain>; do
     git --no-pager grep -nE '<!--[^>]*(vNext|v[0-9]+|future|upcoming|TODO)' \
       "origin/$b" -- 'standard/*.md' | sed "s|^origin/$b:||" \
       > /tmp/vnext-baseline-$b.txt || true
   done
   ```

   The grep is intentionally loose (false positives are fine — Phase 4 surfaces them for human triage; nothing is auto-applied).

## Procedure

Before starting, confirm the chain with the user and show which branches will be touched. Confirm the pre-flight artifacts above are captured. Then for each branch in order:

> **Per-version sweep.** Although the steps below are written per *branch*,
> execute them as a serial sweep that is **atomic per version**: for version
> N, finish `draft-vN` (Steps A–B.8) **and** finalize `alpha-vN` (Step D)
> before touching `draft-v(N+1)`. Do not run ahead down the chain — the
> ordering invariant above forbids building on an unfinalized alpha.

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
   - Wait until checks pass (`gh pr checks <num> --watch` is acceptable, with a reasonable timeout — if it doesn't go green within a few minutes, stop and ask the user).
   - The auto-PR's checks include the `StandardAnchorTags` job. If that job reports any `TOC002` ("`<ref>` not found") diagnostic, **stop** even if `gh pr checks` reports the PR overall as mergeable — broken cross-references must be resolved before merging.
   - Merge it with a **merge commit**: `gh pr merge <num> --merge --delete-branch`.
   - `git pull --ff-only` on the branch.
5. If not found and this is the starting branch, ask the user whether to wait, run the tools locally (`tools/run-smarten.sh`, `tools/run-section-renumber.sh`), or proceed without them. Do not run tools locally without explicit consent.
6. If not found on a downstream branch (it should appear after the merge in step B below), continue.

### Step B — Merge from previous branch (skip on the starting branch)

1. Still on `<branch>`, run:

   ```bash
   git merge --no-ff origin/<previous-branch> \
     -m "Post-meeting propagation: merge <previous-branch> into <branch>"
   ```

2. **Conflicts:**
   - **Never blanket "take upstream"** on a conflict whose hunks touch
     prose in `standard/*.md`. Doing so can silently resurrect text the
     committee deleted on the downstream branch in this cycle or a prior
     one.
   - **Never use `git checkout --theirs <file>` or `git checkout --ours <file>`**
     to resolve conflicts — these commands replace the *entire file* with
     one side, not just the conflicted hunks, silently discarding all
     non-conflicting changes from the other side. Resolve each hunk
     individually in the conflict markers instead. If you accidentally
     run `checkout --theirs/--ours`, recover with `git checkout -m <file>`
     to restore the conflict markers.
   - For each conflicted hunk in `standard/*.md`:
     a. Cross-check the hunk's file and line range against the deletion inventory captured in Pre-flight (`/tmp/starting-deletions.patch`) **and** against any prior `Post-meeting propagation:` merges on the downstream branch (`git log --grep '^Post-meeting propagation:' -p origin/<branch> -- <file>`).
     b. If the downstream side intentionally removed the text, keep the deletion and re-apply only non-overlapping upstream edits to the surrounding region.
     c. If unclear, **abort the merge** (`git merge --abort`) and report to the user with the file, line range, and both sides of the hunk.
   - Mechanical "take upstream" is acceptable **only** for:
     - regenerated `standard/grammar.md`,
     - TOC blocks below the `<!-- The remaining text is generated by a tool. Do not hand edit -->`marker,
     - the generated TOC in `standard/README.md`.
   - For any conflict in `tools/`, `.github/`, or any prose conflict you are not 100% sure is mechanical: **abort the merge** and stop.
3. After conflicts are resolved (or if the merge was clean), **do not push yet**. Run Steps B.5 and B.6 below first.
4. **Section-number drift in alpha branches.** Alpha branches often have
   additional sections inserted (e.g. new feature clauses), which shift
   subsequent section numbers. When merging upstream changes into an alpha
   branch, check for duplicate section headers (e.g. two `## 14.5` headings)
   caused by the merge combining the upstream's numbering with the alpha's
   shifted numbering. The StandardAnchorTags tool will crash with
   `System.InvalidOperationException: Duplicate section header` if this
   happens. Fix by renumbering the duplicated header to its correct value.

### Step B.5 — Cross-reference validation

Run the section-renumber tool in dry-run mode against the post-merge working tree to surface broken or drifted cross-references *before* pushing:

```bash
( cd tools && dotnet run --project StandardAnchorTags -- \
    --owner dotnet --repo csharpstandard --dryrun )
```

1. **Broken references.** Any *new* `TOC002` ("`<ref>` not found") diagnostic that was not already present on `upstream/<branch>` before the merge is a **hard stop**. Compare the tool output against a pre-merge run (or `git stash && run && git stash pop`) to distinguish new from pre-existing. Pre-existing TOC002s from incomplete feature PRs on alpha branches are expected and should be reported but do not block the merge.
2. **Concept drift.** Count how many section numbers the dry run would change (lines beginning with `§` in the tool's diff output). If more than ~25 sections shift, sections have drifted enough that surviving cross-references like `§X.Y (Foo)` may now point at a different concept — the renumber tool fixes the *number* but cannot detect when the *concept* (parenthetical name, surrounding prose) is now wrong. Pause, list the affected references to the user, and proceed only on explicit confirmation.
3. The actual renumber/grammar regeneration runs in the auto-PR opened by `update-on-merge.yaml` after push (Step A on the next iteration); do not commit dry-run output.

### Step B.6 — Future-version comment inventory

Diff the current branch's HTML comments mentioning future versions
against the Pre-flight baseline to surface anything new this propagation brought in (these are notes the committee left for upcoming versions and need to be routed to the appropriate feature PRs by `post-meeting-rebase-prs.prompt.md`):

```bash
git --no-pager grep -nE '<!--[^>]*(vNext|v[0-9]+|future|upcoming|TODO)' \
  HEAD -- 'standard/*.md' | sed 's|^HEAD:||' > /tmp/vnext-current-<branch>.txt
diff /tmp/vnext-baseline-<branch>.txt /tmp/vnext-current-<branch>.txt \
  > /tmp/vnext-delta-<branch>.txt || true
```

> Strip the `HEAD:` prefix with `sed` so diffs against the baseline
> (which uses `upstream/<branch>:` as prefix) don't produce false
> positives on every line.

For each *new* comment in the delta, record:

- file path and line number,
- full comment text,
- branch where it appeared (`<branch>`),
- best-guess target version parsed from the comment (e.g. `v10` from `<!-- v10: tighten wording -->`); leave blank if not parseable.

Persist the accumulated list across the whole run. Include it in the final report so `post-meeting-rebase-prs.prompt.md` can consume it. **Do not edit `standard/*.md` to remove or apply these comments.**

### Step B.7 — Push

1. Push: `git push origin <branch>`.
2. The push triggers `update-on-merge.yaml`, which will open a fresh auto-PR. Loop back to Step A for this branch before moving on.

### Step B.8 — Cheap re-rebase of moved `draft-vN` feature PRs (draft branches with open feature PRs)

This implements step (3) of the per-version sweep. If Step B moved
`draft-vN` (committee changes landed on it this cycle), the vN feature PRs
that **Phase A** (`post-meeting-rebase-prs.prompt.md`) already rebased now
have a stale base.

- Do the **cheap re-rebase of ONLY the vN PRs whose base moved** and
  regenerate their `.patch` (Phase A machinery — `git rebase upstream/draft-vN`
  then `git format-patch upstream/draft-vN..<headRefName>`). Do **not**
  re-rebase PRs whose base did not move.
- These refreshed patches are the input to Step D's `alpha-vN` rebuild. The
  patches Phase B applies to the alpha are always the freshest
  post-propagation ones, not the Phase-A starting patches.

### Step D — Rebuild `alpha-vN` from the fresh feature patches (alpha branches only — Option 1, surgical)

**This is the step that was previously unprompted — its absence is what let
the ordering slip and produced the 2026-07 double-propagation cascade.** Run
it whenever `<branch>` is an alpha branch (`alpha-vN` / `vN-alpha`), after
Step B has propagated the committee changes into it and **BEFORE** the chain
advances to the next draft.

`alpha-vN` must equal **finalized `draft-vN` + the fresh vN feature-PR
patches**, built in ONE pass:

1. **Confirm the fresh per-PR patches for version N are current.** Phase A
   produced them and Step B.8 refreshed any whose base moved. If in doubt,
   regenerate them (`git format-patch upstream/draft-vN..<headRefName>`).
2. **Apply each fresh vN patch surgically onto the EXISTING `alpha-vN`
   structure** — wording-only / subtractive hunks. **Never** rebuild
   `alpha-vN` from scratch or reorganize its structure (Option 1; see the
   v10→v11 structural-divergence trap). A from-scratch structural reorg
   amplifies churn into every downstream branch.
3. **Watch the renumber concept-drift trap.** The renumber tool keeps a
   `§NUMBER` and rewrites the ANCHOR to whatever concept now sits at that
   number, silently corrupting cross-refs while the dry-run reports **no**
   error. For each drifted ref, read the intended concept from the merged
   anchor slug, find that concept's actual number on this branch, and set
   BOTH the number and the anchor manually.
4. **Dry-run validate** (alpha branches get no auto-PR, so do NOT commit
   renumber/grammar output — revert it after validating):

   ```bash
   ( cd tools && dotnet run --project StandardAnchorTags -- \
       --owner dotnet --repo csharpstandard --dryrun )
   ```

   Any *new* `TOC002` diagnostic relative to the pre-rebuild baseline is a
   **hard stop**.
5. **Finalize before advancing.** Only when `alpha-vN` carries BOTH the
   committee changes AND the fresh vN patches may the sweep proceed to
   `draft-v(N+1)` (whose Step B merges from this now-finalized `alpha-vN`).

### Step C — Move to the next branch

Advance the per-version sweep. Before starting `draft-v(N+1)`, confirm
`alpha-vN` is finalized per Step D — **the ordering invariant forbids building
`draft-v(N+1)` on an unfinalized alpha.** Repeat A–D for the next branch in
the chain. Continue until the chain is done.

## Reporting

When finished (or when stopped on a conflict), produce a short report:

- For each branch: action taken (auto-PR merged, propagation merge SHA, skipped).
- For each version N: whether `alpha-vN` was **finalized** (committee changes + fresh vN patches applied surgically, dry-run clean) before `draft-v(N+1)` was started, and the list of vN patches applied (Step D / Step B.8).
- Any branches where you stopped and why.
- The deletion inventory captured in Pre-flight (file list + originating PR numbers).
- The accumulated future-version comment inventory from Step B.6 (file:line, comment text, source branch, best-guess target version). Hand this to `post-meeting-rebase-prs.prompt.md`.
- A reminder to the user to run `post-meeting-rebase-prs.prompt.md` next.

## Safety rules

- Never force-push to any `draft-v*` or `v*-alpha` branch.
- Never use `git reset --hard` on a tracked branch.
- Never merge any PR other than the automated renumber/grammar PR.
- If `gh` shows the auto-PR's checks failing, stop and ask the user.
