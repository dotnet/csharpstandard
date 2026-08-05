# ECMA TC49/TG2 New Member Orientation

This document is intended to help new members of TG2 get up to speed.

## Contacts

- Anything administrative or editorial: Rex
- GitHub-related: Bill or Jon

## TG2 Meetings

We usually meet via videoconference for two hours every four weeks (but not in August).

## C# Spec Format and Home

Some years ago, we moved the spec source from MS Word to GitHub/md, and later the repo was made an open-source project under the .NET Foundation umbrella.

## Committee Officers

- Convener (currently Jon), who has a 1-year term ending at the annual TC49 business meeting, usually held in September. The convener produces each meeting agenda and generally manages the work of the TG.
- Administrator (currently Rex, who wears a number of hats)
  - Project editor: When we moved from Word to GitHub/md, this role changed, but Rex still monitors the spec for compliance wording and formatting.
  - Meeting secretary: Posts draft minutes within 12 hours of a meeting's end. A week later, all corrections/improvements are incorporated and the final draft becomes a numbered document. Also manages the time allocated for each topic during a meeting.
  - Chair of ECMA TC49: This keeps him in touch with the Ecma Secretary General, the bi-annual General Assembly, and the associated Ecma Executive Committee (ExeComm).
  - Ecma liaison to ISO/IEC JTC/1 SC22 (programming languages, tools, and environments): For TC49 (C#, CLI, Eiffel) and TC39 (ECMAScript).
  - Background task: Rex has been creating formal feature specs from [MS’ specs](https://github.com/dotnet/csharplang/tree/main/proposals) for V9–14. The V9 ones are already in the repo as Draft PRs. The formal feature specs for versions beyond V9 are kept offline by Rex (with Jon and Bill also having copies). When V8 is completed and we start work on the V9 Draft PRs, Rex will make the V10 Draft PRs available.

## Official Document Home

All meeting agendas and minutes are assigned numbers by Ecma admin staff, and are posted to the Ecma-hosted TG2 website to which all members have access. Draft versions of these documents are posted privately to members.

## The Standard's Process and Submission/Publication Calendar

Initially, for a given version TG2 produced a final spec that was adopted by Ecma and given an offical number. Then the year-long process of Fast-Tracking it to ISO began. That resulted in at least a few changes, requiring a new Ecma version to be adopted to match the ISO version.

To avoid that extra ISO step and to eliminate the 1-year delay, in December 2022, we created a 4-page "wrapper standard" called "C# Specification Suite (ISO/IEC IS 20619 and ECMA-422)," which has no technical content of its own, but rather includes via undated normative references the latest Ecma C# standard, ECMA-334, and ECMA-335, *Common Language Infrastructure (CLI): Partition IV: Profiles and Libraries, https://www.ecma-international.org/publications-and-standards/standards/ecma-335/*. As such, we no longer Fast-Track the Ecma spec to ISO.

Ordinarily, Ecma approves new specs at its June and December General Assemblies (GAs). To get considered at one of those, we need to have a feature-complete and almost technically complete spec ready by mid-March or mid-September, respectively. If we submit a new version at any other time, rather than have it voted on via a letter ballot, it typcially waits until the next GA. That said, once TG2 completes a version it typically starts work on next one, as Draft PRs for that are already available.

If errors are found in a published spec, we do ***not*** make available a corrected version; corrections simply go into the next version.

## C# Formal Grammar

Some time ago, we moved from an eBNF form to [ANTLR](https://en.wikipedia.org/wiki/ANTLR) notation for the grammar.

While the grammar is written in ANTLR notation the ANTLR parser generator itself cannot be used to build a parser directly based on it. This is due either to the use of grammar features the generator does not support, or the need to use non-grammar features of the ANTLR generator to support them. An example of the former is the C# grammar’s use in a few placers of mutual left recursion, which the ANTLR generator does not support. An example of the latter is C#’s interpolated strings which, like in many languages, are a “language within a language”. Standard *implementation* techniques exist to support such features, which as such are outside the scope of the C# Standard.

We have an internal tool to extract the grammar from the spec, apply ANTLR generator specific implementation techniques, validate the grammar, and run grammar tests. (See [antlr-testing](https://github.com/ECMA-TC49-TG2/antlr-testing/blob/v2/ReadMe.md).)

## Examples in the spec

We have tools to extract the examples and compile them and execute them. (See https://github.com/dotnet/csharpstandard/blob/draft-v8/tools/README.md.)

## Status of Features in the Current Spec Draft

For a given version *X*, see file v*X*-feature-tracker.md in https://github.com/dotnet/csharpstandard/tree/draft-v8/admin.
