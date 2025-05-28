# Sample: Element access

Uses modification set Rules which inherits from Base the semantic check for the LHS of
*element_access* & *null_conditional_element_access* not being an *array_creation_expression*
unless it includes an *array_initializer*, or a *stackalloc_expression* unless it includes
a *stackalloc_initializer*.

The sample contains both valid and invalid samples, the semantic errors the latter
produce are checked – the reference errors are in `Reference/sample.stderr.txt`

The check is a semantic one only, the code still parses successfully and the parse tree
is checked for correctness as with other samples.