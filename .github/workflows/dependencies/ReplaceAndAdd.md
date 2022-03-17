# Replacements & Additions

A “replacement & additions” file can be passed to `BuildGrammar`’s `--modification-file`/`-m`
option and contains replacement and additional rules to be added to: verify/test a grammar,
explore/test new language features, etc. It is a markdown (`.md`) file which follows the same
conventions as those used for the Standard.

The file is processed by `BuildGrammar` similarly to other markdown files; clauses are
noted and text in ```` ```ANTLR ... ``` ```` code blocks is “parsed” (being generous here);
as ANTLR rules, mode commands, and comments (and nothing else, don’t go trying to set options
or other stuff); and added to the corresponding section in the produced grammar (`.g4`) file.

The rules add to, or replace, existing rules in the corresponding clause; new rules are added
to the end of the clause’s rule list, replacements occur in-place. Further in the replacement
case the previous rule is kept as a comment, and this happens regardless of the previous rules
clause i.e. a replacement can move a rule to a different clause.

A rule is only a replacement if it changes the rule’s definition or clause.
Rules with are neither additions or replacements are ignored.

> *Note:* This means that a
replacement & additions file can actually be a complete Standard section markdown file
and only the changed rules will be extracted to update the grammar. So while working, say,
on a PR if the original Standard’s section files are passed to `BuildGrammar` as input files,
the section files changed by the PR are passed as modification files, then the resultant grammar will
reflect the PR’s changes with the old rules as comments.

> **Important:** section numbers in replacement & additions files are not maintained by
the automatic section numbering tooling, they **must** be maintained manually.

---

# Verification-Only Replacements & Additions

This set of replacements and additions is the bare minimum required to allow the grammar to verify and run, though
it may not produce the desired parse (that requires at least the use of modes and/or
lexical predicates).

This set can be used as a basic check that the grammar is a correct ANTLR grammar.

---

## Top Level Rule

The Standard’s *compilation_unit* as is will allow garbage at the end of a file, this
rule has an EOF requirement to ensure the whole of the input must be a correct program.

> *Note: The section number makes this the first rule in the grammar, not required but it
has to go somewhere…*

### 0.0.0 Top Level Rule

```ANTLR
// [ADDED] Rule added as the start point
prog: compilation_unit EOF;
```
---

## Discarding Whitespace

The following changes in §7.3.2, §7.3.3 and §7.3.4, add `-> skip` to the “whitespace”
token rules so that are not passed to the parser. This behaviour is implicit in the
Standard.

### 7.3.2 Line terminators

```ANTLR
// [SKIP]
New_Line
    : ( New_Line_Character
      | '\u000D\u000A'    // carriage return, line feed 
      ) -> skip
    ;
```

### 7.3.3 Comments

```ANTLR
// [SKIP]
Comment
    : ( Single_Line_Comment
      | Delimited_Comment
      ) -> skip
    ;
```

### 7.3.4 White space

```ANTLR
// [SKIP]
Whitespace
    : ( [\p{Zs}]  // any character with Unicode class Zs
      | '\u0009'  // horizontal tab
      | '\u000B'  // vertical tab
      | '\u000C'  // form feed
      ) -> skip
    ;

```

---

## Pre-processing directives

This change causes all pre-processor directives to be discarded, they don’t need to be
processed to validate the grammar (processing them would exercise the *implementation*
of the pre-processor, which is not part of the Standard).

### 7.5.1 General

```ANTLR
// [CHANGE] Discard pre-processor directives
PP_Directive
    : (PP_Start PP_Kind PP_New_Line) -> skip
    ;
```

---

## Mutual Left Recursion Removal

All but one mutual left recursive (MLR) group has been removed from the grammar (and we should
strive not to introduce any new ones).

This change resolves the one remaining MLR group by inlining some of the non-terminal
alternatives in *primary_no_array_creation_expression*.

Non-terminals that are inlined are commented out and the inlined body is indented.

This change has not been made to the Standard itself as it makes *primary_no_array_creation_expression*
“uglier” and would obfuscate somewhat the description in the Standard.

As MLR is not supported by ANTLR without this change the grammar would be rejected.

### 12.7.1 General

```ANTLR
// [CHANGE] This removes a mutual left-recursion group which we have (currently?)
// [CHANGE] decided to leave in the Standard. Without this change the grammar will fail.
primary_no_array_creation_expression
    : literal
    | simple_name
    | parenthesized_expression
    // | member_access
        | primary_no_array_creation_expression '.' identifier type_argument_list?
        | array_creation_expression '.' identifier type_argument_list?
        | predefined_type '.' identifier type_argument_list?
        | qualified_alias_member '.' identifier type_argument_list?
    // | invocation_expression
        | primary_no_array_creation_expression '(' argument_list? ')'
        | array_creation_expression '(' argument_list? ')'
    // | element_access and pointer_element_access (unsafe code support)
        | primary_no_array_creation_expression '[' argument_list ']'
    | this_access
    | base_access
    // | post_increment_expression
        | primary_no_array_creation_expression '++'
        | array_creation_expression '++'
    // | post_decrement_expression
        | primary_no_array_creation_expression '--'
        | array_creation_expression '--'
    | object_creation_expression
    | delegate_creation_expression
    | anonymous_object_creation_expression
    | typeof_expression
    | sizeof_expression
    | checked_expression
    | unchecked_expression
    | default_value_expression
    | nameof_expression
    | anonymous_method_expression
    // | pointer_member_access     // unsafe code support
        | primary_no_array_creation_expression '->' identifier type_argument_list?
        | array_creation_expression '->' identifier type_argument_list?
    // | pointer_element_access    // unsafe code support
        // covered by element_access replacement above
    ;
```
