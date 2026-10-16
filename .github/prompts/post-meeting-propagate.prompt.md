---
mode: agent
description: Discover and propagate committee changes one reviewed version at a time, preserving source intent and active feature work.
---

# Monthly post-meeting propagation

Analyze first, confirm permissions with Bill, then execute one version at a time.
Finalized predecessor -> draft -> authorized feature rebases -> existing alpha ->
local review -> approved publication -> verification -> next authorized version.

Use `admin/branch-diagram.md` as conceptual context, but discover actual remote
refs. Do not assume names or counts: `v11-alpha` is a valid existing alpha name.
When working with csharp-squad, use its authoritative
`post-meeting-propagation`, `standard-renumber`,
`csharpstandard-version-validation`, and `csharp-standard-alpha-synthesis` skills.
The companion `post-meeting-rebase-prs.prompt.md` handles only the current
version's authorized sources; do not pre-rebase the entire chain.

## Required authorization

Confirm repository path, authoritative remote/repository, meeting date and merged
PRs, starting branch, endpoint, and per-version permissions:

- draft-only, draft plus features, or draft plus features plus existing alpha;
- excluded feature heads whose authors are actively working;
- historical alpha feature set/order, including intentionally retained closed PRs;
- local commit and remote publication permissions, separately;
- exact validation exceptions approved for this cycle.

Scope changes override earlier permission. No alpha creation, next-version work,
PR merges/comments, workflow dispatches, or protection changes are implied.
Do not retain a version-specific exclusion forever; reconfirm it next meeting.

## 1. Freeze inventory and recover evidence

Record remote fetch/push repository identity, all relevant old OIDs, local refs,
protected checkout, worktrees/statuses, stashes, and open PR metadata
(number/base/head/repository/OID/state/draft/mergeability).
Use isolated review refs/worktrees; never reset or overwrite divergent local heads.
Keep resumable JSON/text ledgers and logs outside the working tree.

Discover both alpha naming forms. Stop on ambiguity. Absence is not permission to
create an alpha. Preserve the actual historically integrated feature set rather
than equating it with today's open-PR list.

Recover interrupted artifacts before repeating research or mutations. A ledger
must identify exact old/new OIDs and completed stages; verify actual state before
resuming. Never apply stale cleanup stashes.

## 2. Establish approved changes and deletion precedence

Inspect the complete merged series, not only its final commit. A rebase merge may
contain several approved commits; capture the correct series boundary.
Build an approved deletion/refactoring inventory, including relevant prior
committee decisions, and retain source PR provenance.

For example, dotnet/csharpstandard#1691 removed separate pattern subsumption and
exhaustiveness sections while retaining general definitions. Stale
dotnet/csharpstandard#1460 edits must not restore those sections during a rebase or
alpha rebuild.

Approved committee wording wins over speculative feature text. Preserve genuine
later-version additions separately. A clean patch, successful rebase, or numeric
link audit does not prove semantic fidelity.

## 3. Finalize the current draft

Start from actual current upstream draft history and incorporate the verified
finalized predecessor. Preserve unique approved draft changes and the version's
compiler/tooling contract, including intentionally isolated legacy fixtures.

Resolve conflicts hunk-by-hunk using approved sources and deletion provenance.
Never blanket-select a conflicted prose file. Separate number relocation from
meaning. Stop for normative ambiguity or unexplained tooling changes.

Finalize draft numbering/grammar with the repository's exact tool when authorized.
From `tools`, with `GH_TOKEN` and `HEAD_SHA` cleared, the command is:

```text
dotnet run --project StandardAnchorTags -- --owner dotnet --repo csharpstandard
```

Require zero placeholders/symbolic section references, correct hierarchy,
concept-correct links, source-derived grammar, and idempotence.
Do not hand-edit generated `standard/grammar.md`.

## 4. Refresh only authorized current-version feature sources

Invoke the companion rebase prompt only after this draft is finalized.
Skip the phase entirely for draft-only versions. Respect all do-not-touch heads.
Freeze/recheck each authorized PR head; do not overwrite author movement.

Repair defects at the earliest authorized source and propagate them forward.
Audit retired sections on sources and alpha, not only conflict markers.
Never invoke `StandardAnchorTags` on feature branches, including dry runs.

## 5. Rebuild the existing authorized alpha

Integrate refreshed feature intent in historical order on an isolated review ref.
Preserve approved retained closed feature content. Compare old alpha, finalized
draft, current feature sources, and committee decisions in overlapping sections.

Complete source/link/format fixes before finalization. Keep contextual-keyword
Markdown table rows at equal cell counts. ANTLR `contextual_keyword` alternatives
use five columns, allowing a final partial row; preserve tokens and their order.

The exact section-renumber tool is the **last source-changing operation on the
alpha**. Run it, then the identical command again and prove byte-idempotence.
Regenerate grammar from source. Later source corrections require repeating this
finalization and affected validation.

Do not build the next draft from a provisional alpha.

## 6. Validate, classify, and hand off for review

Prove actual ordinary compiler version, generated-project counts, legacy-fixture
isolation, lint, anchors/TOC, source fidelity, examples, converter, tool tests,
grammar, and applicable PR checks.

Separate PASS, exact authorized EXCEPTION, blocked-layer UNAVAILABLE, and
NOT-TRIGGERED results. Pre-existing failures are not automatic waivers. Record
exception authorization, exact diagnostics and input digests at each version.
Revalidate changed inputs; stop for different failures. Do not fabricate harness
outputs or silently modify test contracts.

Provide exact worktree/ref/HEAD, tracked status, diffstat, source fixes, and review
commands. Use `git diff <remote>/<target>` for the complete working-tree delta;
appending `HEAD` omits staged/unstaged corrections. Distinguish committed review
changes from additional working-tree edits. Stop for Bill's review.

## 7. Publish only the reviewed state

After explicit approval, commit only exact approved files with runtime trailers.
Freeze the resulting reviewed OIDs. Re-read remote OIDs/protection immediately
before writing; stop on unexplained movement or lease failure.

Use explicit refspecs and exact
`--force-with-lease=refs/heads/<branch>:<frozen-old-oid>` for every ref, preferably
one atomic authorized version cohort. Drafts/alphas must advance fast-forward.
A lease on a fast-forward is not a history rewrite. Rewritten feature publication
requires explicit permission; no unconditional force or blind lease refresh.

Verify remote equality, ancestry, source PR state/base/head/mergeability, and
protected checkout/worktrees/stashes/refs.
Inspect actual workflow branch/path filters before waiting for CI or bot PRs.
No triggered Actions run means NOT-TRIGGERED, not green CI. Queued third-party
suites are not proof of relevant validation. Dispatches and bot merges require
separate approval.

## Closeout

Persist permissions, topology, provenance, old/new OIDs, exact leases, source
repairs, review evidence, validation classifications, remote equality and
preservation evidence. Record the next-version stop explicitly.

Future-version notes are inventory for human triage, not permission to edit,
remove, or post them. Report optional notifications for approval.
Completion/publication never automatically authorizes the next version.
