# 1 Heading

Rules:

- If `x` is zero and the namespace declaration contains an *extern_alias_directive* or *using_alias_directive* that associates the name `I` with an imported namespace or type, then the *namespace_or_type_name* refers to that namespace or type.
- Otherwise, if the namespaces imported by the *using_namespace_directive*s of the namespace declaration contain exactly one type having name `I` and `x` type parameters, then the *namespace_or_type_name* refers to that type constructed with the given type arguments.
