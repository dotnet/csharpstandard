---
mode: agent
description: Refresh only authorized current-version feature sources after their draft is finalized; preserve committee changes and active author work.
---

# Monthly feature-source refresh

This is the source-rebase step of `post-meeting-propagate.prompt.md`, not a
separate whole-chain rebasing pass. Discover/inventory once; for each version,
finalize its draft first, then refresh only its authorized features. Build the
alpha from those fresh sources before advancing downstream.

## Inputs and permission gate

- Explicit current version and finalized draft ref/OID.
- Repository path and authoritative remote/repository identity.
- Frozen open PR inventory: number/base/head/repository/OID/state/draft status.
- Explicit included sources, excluded active-author heads, and historical alpha
  content/order supplied by the propagation coordinator.
- Protected local refs/worktrees/stashes, artifact directory, validation
  exceptions authorized for this cycle, and separate commit/push permissions.

Draft-only scope skips all feature mutations and validation. Exclusions override
earlier authorization. Fork heads are read-only; comments require approval.
Never merge/close PRs, infer alpha creation, or advance versions.

## 1. Freeze and recheck

Discover actual refs and PRs; no hardcoded counts. Query each target draft with
`gh pr list --repo <owner/repo> --base <draft> --state open --limit 1000` and
capture number, base/head, head repository/owner, head OID, draft status and URL.
If the limit is reached, paginate rather than claiming a complete inventory.

Validate fetch/push remote identity. Record excluded sources without operating on
them. Recheck each included PR's current state/base/head immediately before work.
If an author moved the head, stop that source and request a scope/pin decision.
Do not silently refresh a lease.

Create a new isolated review ref/worktree from the frozen authoritative head.
Never use a failed checkout as permission to reset a divergent user's branch.
Do not apply archived cleanup stashes.

## 2. Rebase source intent onto the finalized draft

Rebase only the included source commits, preserving authored intent and history.
Record original/new commit correspondence and inspect the full feature delta.

Resolve conflicts hunk-by-hunk:

- Approved committee wording/deletions/refactorings supersede stale feature base
  text. Preserve genuine feature additions separately.
- Formatting-only changes still need source evidence; no blanket whole-file
  selection. Binary tooling conflicts require verified version-aware inputs,
  never an unconditional preference for either side.
- Mechanical relocation and additive edits may be reconciled with provenance.
- Normative ambiguity, unknown tooling contracts, or active excluded ownership
  stops for Bill's decision.

Audit resurrection even when a rebase applies cleanly.
Dotnet/csharpstandard#1691 retired separate pattern subsumption/exhaustiveness
sections; do not restore them from dotnet/csharpstandard#1460. General pattern
definitions retained by the committee are intentional.

## 3. Read-only feature audits and validation

**Never run `StandardAnchorTags` on a feature branch**, including dry runs and
idempotence checks. Symbolic authoring placeholders may remain where intentional.
Use read-only heading/link/source audits instead.

Validate actual intended ordinary compiler version, generated project counts,
isolated legacy fixtures, examples, lint, converter, tools, normative grammar
extraction and applicable checks. Compare source intent and approved removals.
Do not hand-edit generated `standard/grammar.md`.

Report PASS, exact authorized EXCEPTION, and blocked-layer UNAVAILABLE separately.
Pre-existing failures require explicit current-cycle approval and exact
diagnostic/input proof. A changed failure stops; do not widen the waiver.
Do not alter a harness contract or fabricate reference outputs.

Repair a feature defect here before rebuilding its alpha when authorized.
Excluded sources require a user decision; do not patch them indirectly.

## 4. Refresh provenance and local review

Record original/new OIDs, commit correspondence, intended feature delta,
base OID, conflict decisions, patch evidence, timestamps, and validation results.
Patches are evidence of feature intent, not instructions to replay stale
synchronization/base text on an alpha.

Recheck state/base/head/draft/repository metadata. Provide exact review
worktree/ref/HEAD and status. A working-tree comparison against the authoritative
head includes uncommitted corrections; a diff ending in `HEAD` does not.
Freeze the actual reviewed state and stop before unauthorized commits/pushes.

## 5. Approved publication only

After Bill approves, stage only exact reviewed files and include runtime trailers
in any new commits. Re-read each remote head and protection immediately before
publishing. Push only explicit authorized refs, with:

```text
--force-with-lease=refs/heads/<head>:<frozen-old-head-oid>
```

Prefer the propagation coordinator's atomic per-version cohort. Never use an
implicit lease, unconditional force, or blind retry. Author movement invalidates
the lease; do not overwrite it. No writes to forks or excluded heads.

Verify exact remote equality and source PR state/base/head/draft/mergeability.
Settle applicable CI under exact exceptions before reporting terminal results.
Refresh alpha inputs from these verified sources, not earlier stale patch pins.

## Optional notifications

Report fork rebase requests, alpha drift, and future-version comments as proposed
human follow-ups. Post comments only with explicit approval and deduplicate them
with persistent markers. Notifications are not input to alpha reconstruction and
never authorize alpha edits, source rewrites, PR merges, or closures.

## Reporting

Group current-version results by target draft: refreshed, excluded, author-moved,
fork/read-only, or blocked. Include exact old/new OIDs, source provenance,
validation classifications, protection/preservation evidence and review commands.
No completion implies permission for the next version.
