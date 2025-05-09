# Sample: Type argument list disambiguation – Query expressions

Uses modification set Rules which implements the disambiguation algorithm
to determine whether a sequence of tokens is a *type_argument_list* or some expression
when it occurs within a *query_expression*.

Disambiguation is applied when a *type_argument_list* appears immediately before
a contextual query keyword within a query expression. This can happen with
those query clauses which end in an *expression* (e.g. *where_clause*)
and those with *expression* followed by a contextual keyword within the clause
itself (e.g. *join_clause* where the keywords `on` and `in` follow *expression*s).

This is essentially the same process as for disambiguating within expression
context, but with the addition of additional following tokens within the query context.
