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

This set of replacements and additions is the bare minimum required to allow the grammar
to verify and run, though it may not produce the desired lex and parse (that requires at
least the use of modes and/or lexical predicates).

Pre-processing directives are skipped like whitespace, however lexing confirms the lexical
grammar is valid.

This set can be used as a basic check that the grammar is a valid ANTLR grammar.


---

## Mutual Left Recursion Removal

All but one mutual left recursive (MLR) group has been removed from the grammar (and we should
strive not to introduce any new ones).

This change resolves the one remaining MLR group by inlining some of the non-terminal
alternatives in *primary_no_array_creation_expression*.

Non-terminals that are inlined
are commented out and the inlined body is indented.

This change has not been made to the Standard itself as it makes *primary_no_array_creation_expression*
“uglier” and would obfuscate somewhat the description in the Standard – both
subjective reasons of course...

As MLR is not supported by ANTLR without this change the grammar would be rejected.

### 12.8.1 General

```ANTLR
// [CHANGE] This removes a mutual left-recursion group which we have (currently?)
// [CHANGE] decided to leave in the Standard. Without this change the grammar will
// [CHANGE] fail to verify.
# Expect
primary_no_array_creation_expression
    : literal
    | interpolated_string_expression
    | simple_name
    | parenthesized_expression
    | tuple_expression
    | member_access
    | null_conditional_member_access
    | invocation_expression
    | element_access
    | null_conditional_element_access
    | this_access
    | base_access
    | post_increment_expression
    | post_decrement_expression
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
    | pointer_member_access     // unsafe code support
    | pointer_element_access    // unsafe code support
    | stackalloc_expression
    ;
# ReplaceWith
primary_no_array_creation_expression
    : literal
    | interpolated_string_expression
    | simple_name
    | parenthesized_expression
    | tuple_expression
    // | member_access
        | primary_no_array_creation_expression '.' identifier type_argument_list?
        | array_creation_expression '.' identifier type_argument_list?
        | predefined_type '.' identifier type_argument_list?
        | qualified_alias_member '.' identifier type_argument_list?
    // | null_conditional_member_access
    	| primary_no_array_creation_expression '?' '.' identifier type_argument_list? dependent_access*
    	| array_creation_expression '?' '.' identifier type_argument_list? dependent_access*
    // | invocation_expression
        | primary_no_array_creation_expression '(' argument_list? ')'
        | array_creation_expression '(' argument_list? ')'
    // | element_access and pointer_element_access (unsafe code support)
        | primary_no_array_creation_expression '[' argument_list ']'
    // | null_conditional_element_access
        | primary_no_array_creation_expression '?' '[' argument_list ']' dependent_access*
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
    | stackalloc_expression
    ;
```


---

## Interpolated strings

The lexical rules for interpolated strings are context-sensitive and are not ANLTR-ready in the Standard
as how such rules are handled is an implementation detail, e.g. using ANTLR modes.
Here we just define one token in terms of another to remove the overlap warnings.

### 12.8.3 Interpolated string expressions

```ANTLR
// [CHANGE] This allows the grammar to verify without warnings, it does NOT correctly
// [CHANGE] parse interpolated strings – that requires modes  and/or lexical predicates.
// [CHANGE] Note: Interpolated strings are properly parsed in Base and other sets.
# Expect
Interpolated_Verbatim_String_End
    : '"'
    ;
# ReplaceWith
Interpolated_Verbatim_String_End
    : Interpolated_Regular_String_End
    ;
```
