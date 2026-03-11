# Feature PR workflow process

The purpose of this PR workflow process is to focus on the right review at the right time for each feature. We need enough detail in the feature PR to provide focused feedback, but we also want to keep the discussion focused on the decisions to drive the PR forward.

The goal is to provide two sets of gates through the review process: First, agree on the overall structure and edits necessary to specify the new feature. Second, refine the spec updates and finalize before merging.

## Assignee review and update

A committee member takes ownership of the draft PR and self-assigns it. The *assignee* reviews and modifies the initial PR. At the assignee's discretion, they can tag other members for help or opinion on the overall direction of the PR.

Once the assignee has a good first draft, it's ready for the [First Reading](#first-reading). As part of getting ready for the first reading, the assignee should provide a summary of the PR to guide reviewers. This should cover:

- Roadmap of the PR.
- Open questions on how to spec the behavior.
- Comments that explain decisions on spec language.

## First reading

The *first reading* is when the committee as a whole discusses the overall direction of the PR. This should focus on large-scale direction and concepts rather than nits in the specification language. (Although we likely can't help ourselves from pointing out the smaller issues we find as we read the PR.)

The assignee leads the discussion based on the PR summary and comments made by other committee members prior to the meeting.

The outcome of the first reading is one of:

- Direction approved, move on to final edits and [Second reading](#second-reading).
- Direction or content needs major work. Bring a revised PR back for a new first reading. (Task list comes from meeting review comments.)

The goal for this review is to minimize major rewrites to large new issues discovered later in the process. On the rare occasion that a feature PR is quite small, the first reading and second reading can be combined. 

## Second reading

The assignee addresses concerns raised during the first reading, and brings the PR to the next meeting for a second reading and final review. At this point cross references and style should be addressed.

The outcome of the second reading is one of:

- Merge after applying suggestions.
- Agree to merge offline after other edits.

In addition, in the case of major issues identified only at the second reading, the committee agrees to open issues for outstanding work related to the feature.

> The lack of a mechanism for a complete rejection of a feature PR at the second reading stage is intentional. The goal is to catch issues at that scale in the first reading. The plan is that major issues that slip through become issues to be addressed in the same release of the standard.
