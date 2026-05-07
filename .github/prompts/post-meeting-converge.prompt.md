---
mode: agent
description: (Stub) Converge a finished version's draft and alpha branches.
---

# Post-meeting: converge draft and alpha for a completed version

> **Status: not finalized.** The exact end-state for converging
> `draft-vN` and `vN-alpha` when version N is finished has not been
> decided by the committee. **Do not run this prompt yet.** Ask the
> user for the intended target state first, then update this prompt
> with the agreed procedure before executing anything.

## Open questions to resolve before implementing

- When v8 is finalized, does `draft-v8` become `standard-v8`, and is
  `v8-alpha` deleted? Or is `v8-alpha` merged into `draft-v8` first
  (and if so, which way do feature-PR conflicts resolve)? Note that
  v8 currently has no alpha branch.
- What renames or branch-protection updates are needed?
- What needs to happen to in-flight feature PRs whose base was the
  retired `vN-alpha`?
- Is there a Word/PDF artifact step to run at convergence?
- How do we reconcile feature PRs whose `draft-vN` head differs from the
  version already merged into `vN-alpha`? See the alpha-drift handling
  in `post-meeting-rebase-prs.prompt.md` (Step 0). Convergence must
  define whether the canonical text is the alpha version, the draft
  version, or requires per-file manual reconciliation.
- How do we audit that no committee-approved deletion on `vN-alpha` was
  silently resurrected during the version's propagation history?
  Suggested: a final `git log --diff-filter=D` review against the
  `standard-v(N-1)` baseline at convergence time.
- Disposition of any unrouted or still-unresolved vNext HTML comments
  remaining in `standard/*.md` when version N converges (see Step 0.5
  of `post-meeting-rebase-prs.prompt.md`). Do they get applied,
  deferred to v(N+1), or deleted?

## Outline (to be fleshed out once decided)

1. Verify all feature PRs targeting `vN-alpha` and `draft-vN` are
   merged or reassigned to a later branch.
2. Reconcile `draft-vN` and `vN-alpha` per the committee's decision.
3. Rename / retire branches as decided.
4. Update `admin/branch-diagram.md`, `README.md`, and the propagate
   prompt's chain to remove the retired branches.
5. Update branch protection rules and
   `.github/workflows/update-on-merge.yaml`.

When the semantics are decided, replace this stub with a full procedure
modeled on `post-meeting-propagate.prompt.md`.
