---
mode: agent
description: Phase B — serial per-version sweep that propagates committee changes and finalizes each alpha (committee changes + fresh patches) before advancing.
---

# Post-meeting: propagate changes through draft & alpha branches (Phase B)

You are propagating the changes merged into the current "starting" draft branch (after a TC49-TG2 committee meeting) forward through every future draft and alpha branch. See `admin/branch-diagram.md` for the rationale.

## Phase B of the two-phase post-meeting workflow

The monthly post-meeting work is split into two phases (see
`post-meeting-rebase-prs.prompt.md` for **Phase A**):

- **Phase A1 — feature-PR preparation (runs first).** Dynamically enumerate
  the open feature PRs for the discovered draft branches, report the count,
  rebase or notify them as appropriate, and produce a patch manifest. Each
  manifest entry pins the PR number, base branch, head SHA, patch path, and
  per-PR UTC cutoff time.
- **Phase B — this prompt: a single serial sweep up the chain, atomic per
  version.** For each version N in chain order, (1) propagate the
  committee-merged changes into `draft-vN`, (2) real-renumber `draft-vN`
  (watch the concept-drift trap), (3) cheap re-rebase only the vN PRs whose
  base moved in step 1 and update their manifest pins, (4) **reconstruct the
  version's alpha branch** by surgically applying the fresh feature deltas
  onto its existing structure, validate, then (5) advance so
  `draft-v(N+1)` is based on the now-finalized discovered alpha branch.
- **Phase A2 — notification follow-up (runs after Phase B).** Consume Phase
  B's deletion and future-version comment inventories to post alpha-drift
  and routing notices. Phase A2 does not feed alpha reconstruction.

Do not interpret the phase names as a repeating A/B cycle. The chronology is
**A1 → B → A2**. Phase B may call the narrowly scoped Phase A rebase
machinery only to refresh PRs whose base moved during Phase B.

### ⚠️ Ordering invariant (the crux — do not violate)

**Never propagate or build anything downstream of version N's discovered
alpha branch until that branch is finalized with BOTH the committee-merged
changes AND the fresh feature-PR patches.** In one sentence: finalized alpha
= finalized `draft-vN` + fresh vN feature deltas, constructed in **ONE
pass**, before `draft-v(N+1)` is built on it.

This ordering reduces the risk of a second propagation cascade: propagating
the whole chain before refreshing alpha feature content can make downstream
branches depend on stale alpha text. It does not eliminate downstream drift.
Every later draft and alpha still requires the raw-reference, semantic, and
head-movement checks defined below.

## Observed failure modes this workflow must detect

- A numbered heading can retain a stale symbolic anchor such as `§xx` or
  `§some-placeholder`.
- A feature PR can move after the patch cutoff, making the recorded patch
  stale even when it still applies cleanly.
- A downstream conflict resolution can preserve older normative structure
  or wording instead of the intended later-version language.

## Assumptions

- The committee secretary has already merged all approved PRs into the starting branch (default: `draft-v8`). Do **not** merge any other open PRs.
- The `.github/workflows/update-on-merge.yaml` workflow will open an automated PR titled "Automated Section renumber and grammar extraction" on each branch that receives a push. These auto-PRs **must be merged** before the next propagation hop.
- Auto-PRs may show a `BLOCKED` merge state due to branch protection rules requiring review approval — this is normal and not a CI failure. Wait for checks to pass, then merge.
- You have `git` and `gh` available, and push permission to this repo.

## Discover the propagation chain

Do not rely on a fixed list or assume every alpha is named `alpha-vN`.

1. Fetch and list the actual remote branch names:

   ```bash
   git fetch upstream --prune
   git for-each-ref refs/remotes/upstream \
     --format='%(refname:strip=3)' \
     | grep -E '^(draft-v[0-9]+|alpha-v[0-9]+|v[0-9]+-alpha)$' \
     | sort -V
   ```

2. Starting with the user-supplied draft branch (default `draft-v8`),
   enumerate later `draft-vN` branches by numeric version. For each version,
   discover whether the remote has `alpha-vN` or `vN-alpha`; use the one that
   exists. `v11-alpha` is valid and must not be rewritten as `alpha-v11`.
   Stop if both names exist for one version or if the branch topology is
   ambiguous.
3. Build the ordered chain as the starting draft, then each later draft,
   followed by its discovered alpha when one exists. Confirm the order
   against `admin/branch-diagram.md` and remote ancestry. Report the exact
   discovered chain before changing anything.

> **Walk this chain as a per-version, atomic-per-version sweep.** For each
> version N, finish `draft-vN` (propagate committee changes + renumber +
> cheap re-rebase of moved vN PRs) **and** finalize the discovered alpha
> branch (rebuild it from the fresh vN patches — Step D) BEFORE advancing to
> `draft-v(N+1)`. The discovered alpha branch (`alpha-vN` or `vN-alpha`) is
> never just a plain merge of its draft: it must also receive the fresh
> feature-PR patches in the same pass, per the ordering invariant above.
>
> Inspect `.github/workflows/update-on-merge.yaml` and report which
> discovered branches are covered by `on.push.branches`. Do not assume an
> auto-PR will appear on an uncovered branch. Alpha branches are commonly
> uncovered, so their validation must be completed locally before advancing.

If the user names a different starting branch, start there and propagate to
every later branch in the discovered chain.

## Pre-flight (run once, before walking the chain)

These artifacts are referenced by Step B (conflict resolution) and Step B.6
(future-version comment inventory). Capture them up front and persist them
for the duration of the run.

1. **Run directory and Phase A manifest.** Create a persistent, untracked run
   directory under `.git`, not a temporary system directory:

   ```bash
   RUN_ID="$(date -u +%Y%m%dT%H%M%SZ)"
   RUN_DIR="$(git rev-parse --git-path "post-meeting/$RUN_ID")"
   mkdir -p "$RUN_DIR"
   ```

   Load Phase A1's feature-PR manifest. Require one record per dynamically
   enumerated feature PR, including PR number, base branch, head SHA, patch
   path, and UTC cutoff time. Report the manifest's resulting PR count. Stop
   if the manifest is missing, has duplicate PR numbers, or its count differs
   from a fresh enumeration of open PRs on the discovered target draft
   branches.

2. **Baseline.** Determine the SHA on each branch in the chain at the
   *previous* propagation. A reliable proxy is the most recent merge commit whose subject begins with `Post-meeting propagation:` on that branch (`git log --grep '^Post-meeting propagation:' -1 --format=%H origin/<branch>`). Record `BASE_<branch>` for each branch. If a branch has no such commit yet, fall back to the branch point and warn the user.

3. **Deletion inventory** for the starting branch. Identify text the
   committee removed in this cycle so Step B can recognize resurrection:

   ```bash
   # Files where lines were removed since the previous propagation:
   git log "$BASE_<starting>"..origin/<starting> --diff-filter=MD \
     --name-only --pretty=format: -- standard/ | sort -u

   # Hunks (negative lines) for review:
   git log "$BASE_<starting>"..origin/<starting> -p -- standard/ \
     > "$RUN_DIR/starting-deletions.patch"
   ```

   Show the user the list of PRs merged this cycle (`gh pr list --base <starting> --state merged --search 'merged:>=<last-meeting-date>'`) and ask which performed substantive prose removals. Persist that list.

4. **vNext-comment baseline** for every branch in the chain. Snapshot the current set of HTML comments in `standard/*.md` mentioning a future version, so Step B.6 can diff against it after each merge:

   ```bash
   for b in <chain>; do
     git --no-pager grep -nE '<!--[^>]*(vNext|v[0-9]+|future|upcoming|TODO)' \
       "origin/$b" -- 'standard/*.md' | sed "s|^origin/$b:||" \
       > "$RUN_DIR/vnext-baseline-$b.txt" || true
   done
   ```

   The grep is intentionally loose (false positives are fine — Phase A2
   surfaces them for human triage; nothing is auto-applied).

## Procedure

Before starting, confirm the chain with the user and show which branches will be touched. Confirm the pre-flight artifacts above are captured. Then for each branch in order:

> **Per-version sweep.** Although the steps below are written per *branch*,
> execute them as a serial sweep that is **atomic per version**: for version
> N, finish `draft-vN` (Steps A–B.8) **and** finalize its discovered alpha
> branch (Step D) before touching `draft-v(N+1)`. Do not run ahead down the
> chain — the ordering invariant above forbids building on an unfinalized
> alpha.

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
     a. Cross-check the hunk's file and line range against the deletion inventory captured in Pre-flight (`$RUN_DIR/starting-deletions.patch`) **and** against any prior `Post-meeting propagation:` merges on the downstream branch (`git log --grep '^Post-meeting propagation:' -p origin/<branch> -- <file>`).
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
2. **Mandatory raw symbolic-reference audit.** Run this independently of
   the tool and save the output:

   ```bash
   rg -n -e '§[a-z][a-z0-9-]*' -e '§xx' -e '#xx' \
     standard --glob '*.md' \
     > "$RUN_DIR/symbolic-refs-<branch>.txt" || true
   ```

   Review every match. A retained placeholder is a hard stop unless the
   branch intentionally permits that exact pre-merge placeholder and it is
   recorded in the report. This raw audit is mandatory because
   `StandardAnchorTags` and its `TOC002` diagnostics previously missed
   placeholders that remained in otherwise valid text, including symbolic
   anchors left after headings had already been numbered.
3. **Concept drift.** Count how many section numbers the dry run would change (lines beginning with `§` in the tool's diff output). If more than ~25 sections shift, sections have drifted enough that surviving cross-references like `§X.Y (Foo)` may now point at a different concept — the renumber tool fixes the *number* but cannot detect when the *concept* (parenthetical name, surrounding prose) is now wrong. Pause, list the affected references to the user, and proceed only on explicit confirmation.
4. The actual renumber/grammar regeneration runs in the auto-PR opened by `update-on-merge.yaml` after push (Step A on the next iteration); do not commit dry-run output.

### Step B.6 — Future-version comment inventory

Diff the current branch's HTML comments mentioning future versions
against the Pre-flight baseline to surface anything new this propagation brought in (these are notes the committee left for upcoming versions and need to be routed to the appropriate feature PRs by `post-meeting-rebase-prs.prompt.md`):

```bash
git --no-pager grep -nE '<!--[^>]*(vNext|v[0-9]+|future|upcoming|TODO)' \
  HEAD -- 'standard/*.md' | sed 's|^HEAD:||' > "$RUN_DIR/vnext-current-<branch>.txt"
diff "$RUN_DIR/vnext-baseline-<branch>.txt" "$RUN_DIR/vnext-current-<branch>.txt" \
  > "$RUN_DIR/vnext-delta-<branch>.txt" || true
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
have a stale base. Run this step only after Step A has merged the auto-PR
created by Step B.7, so the feature PRs rebase onto the final draft head for
this version.

- Do the **cheap re-rebase of ONLY the vN PRs whose base moved** and
  regenerate their `.patch` (Phase A machinery — `git rebase upstream/draft-vN`
  then `git format-patch upstream/draft-vN..<headRefName>`). Do **not**
  re-rebase PRs whose base did not move.
- Immediately before rebasing each PR, query `headRefOid` again and compare
  it with the manifest pin. If it moved after the recorded cutoff, stop for
  that PR; do not overwrite or build an alpha from a stale patch. After a
  successful rebase, update that PR's manifest entry with the new head SHA,
  regenerated patch, and a new UTC cutoff time.
- These refreshed patches are the input to Step D's alpha rebuild. The
  patches Phase B applies to the alpha are always the freshest
  post-propagation ones, not the Phase-A starting patches.

### Step D — Surgically reconstruct the alpha from fresh feature deltas

**This is the step that was previously unprompted — its absence is what let
the ordering slip and produced the 2026-07 double-propagation cascade.** Run
it whenever `<branch>` is a discovered alpha branch (`alpha-vN` or
`vN-alpha`), after Step B has propagated the committee changes into it and
**BEFORE** the chain advances to the next draft.

The discovered alpha must equal **finalized `draft-vN` + the fresh vN
feature deltas**, built in ONE pass:

1. **Recheck every PR head after the potentially long propagation run.**
   Query each manifest PR's current `headRefOid` and compare it with the
   pinned SHA and cutoff time. If any head moved, stop alpha reconstruction,
   refresh that PR through Phase A machinery, record a new pin and cutoff,
   and restart validation. Never silently use the older patch.
2. **Treat patches as feature-delta evidence, not commit-replay scripts.**
   Apply the minimal normative delta onto the existing alpha structure.
   Do not replay synchronization or base-merge commits wholesale, and do not
   replace an alpha file with the feature PR's version. Preserve
   branch-specific later-version language and structure unless the feature
   itself intentionally changes it.
3. **Perform a semantic three-way comparison for every high-conflict
   normative section.** Compare:
   - the section in the prior finalized alpha (`BASE_<alpha-branch>`),
   - the section in the finalized target `draft-vN`, and
   - the section at the pinned current feature-PR head.

   Record the intended feature semantics, committee changes, and
   later-version-only language that the reconstructed alpha must preserve.
   Review the resulting alpha against that record. A patch applying cleanly,
   a successful rebase, or correct renumbering is not evidence that the
   normative result is semantically correct. If the three sources cannot be
   reconciled confidently, stop and report the exact section and competing
   text.
4. **Watch the renumber concept-drift trap.** The renumber tool keeps a
   `§NUMBER` and rewrites the ANCHOR to whatever concept now sits at that
   number, silently corrupting cross-refs while the dry-run reports **no**
   error. For each drifted ref, read the intended concept from the merged
   anchor slug, find that concept's actual number on this branch, and set
   BOTH the number and the anchor manually.
5. **Dry-run and raw-reference validate** (alpha branches get no auto-PR, so do NOT commit
   renumber/grammar output — revert it after validating):

   ```bash
   ( cd tools && dotnet run --project StandardAnchorTags -- \
       --owner dotnet --repo csharpstandard --dryrun )
   ```

   Any *new* `TOC002` diagnostic relative to the pre-rebuild baseline is a
   **hard stop**. Also repeat the mandatory raw `rg` audit from Step B.5
   across all `standard/*.md`; any unexplained `§[a-z][a-z0-9-]*`, `§xx`,
   or `#xx` match is a hard stop.
6. **Finalize before advancing.** Only when the discovered alpha carries BOTH the
   committee changes AND the fresh vN patches may the sweep proceed to
   `draft-v(N+1)`. This reduces propagation risk; it does not replace
   downstream semantic and reference verification.

### Step C — Move to the next branch

Advance the per-version sweep. Before starting `draft-v(N+1)`, confirm
the discovered alpha is finalized per Step D — **the ordering invariant
forbids building `draft-v(N+1)` on an unfinalized alpha.** Repeat A–D for the
next branch in the chain. Continue until the chain is done.

## Reporting

When finished (or when stopped on a conflict), produce a short report:

- For each branch: action taken (auto-PR merged, propagation merge SHA, skipped).
- The dynamically enumerated feature-PR count and the manifest cutoff time.
- For each version N: whether its discovered alpha branch was **finalized** (committee changes + fresh vN deltas applied surgically, semantic comparison complete, dry-run and raw-reference audits clean) before `draft-v(N+1)` was started, and the list of vN PR numbers, pinned SHAs, and patches applied (Step D / Step B.8).
- Any PR head that moved after cutoff and how it was refreshed or why work stopped.
- The high-conflict normative sections compared and any unresolved semantic differences.
- Any raw symbolic-reference matches and their disposition.
- Any branches where you stopped and why.
- The deletion inventory captured in Pre-flight (file list + originating PR numbers).
- The accumulated future-version comment inventory from Step B.6 (file:line, comment text, source branch, best-guess target version). Hand this to `post-meeting-rebase-prs.prompt.md`.
- A reminder to run the Phase A2 notification pass in
  `post-meeting-rebase-prs.prompt.md`.

## Safety rules

- Never force-push to any `draft-v*`, `alpha-v*`, or `v*-alpha` branch.
- Never use `git reset --hard` on a tracked branch.
- Never merge any PR other than the automated renumber/grammar PR.
- If `gh` shows the auto-PR's checks failing, stop and ask the user.
