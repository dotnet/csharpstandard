# Sample: Roslyn precedence

This is taken from https://github.com/dotnet/roslyn/issues/10492.

Our grammar produced the correct parse, however it did produce an ANTLR message for one line which had no effect on the resultant parse. This was down to the way ANTR predicates work and our disambiguation predicate for *type_argument_list*. A small change to the predicate addressed this and the harmless message no longer occurs. See Rules.g4 for the details (the comment is longer than the change).
