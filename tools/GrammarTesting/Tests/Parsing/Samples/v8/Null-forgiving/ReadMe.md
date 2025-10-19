# Sample: Null-forgiving expression

Uses modification set Rules which implements the semantic check for the null-forgiving
operator being applied to a null-forgiving expression, including any hidden inside parens.

The sample contains both valid and invalid samples, the semantic errors the latter
produce are checked – the reference errors are in `Reference/sample.stderr.txt`

The check is a semantic one only, the code still parses successfully and the parse tree
is checked for correctness as with other samples.
