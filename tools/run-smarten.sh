#!/bin/bash
set -eu -o pipefail

declare -r SPEC_DIRECTORY=../standard
declare -r EXTENSION=md

declare -a SPEC_FILES=(
    "foreword.md"
    "introduction.md"
    "scope.md"
    "normative-references.md"
    "terms-and-definitions.md"
    "general-description.md"
    "conformance.md"
    "lexical-structure.md"
    "basic-concepts.md" 
    "types.md"
    "variables.md"
    "conversions.md"
    "expressions.md"
    "statements.md"
    "namespaces.md"
    "classes.md"
    "structs.md"
    "arrays.md"
    "interfaces.md"
    "enums.md"
    "delegates.md"
    "exceptions.md"
    "attributes.md"
    "unsafe-code.md"
    "grammar.md"
    "portability-issues.md"
    "standard-library.md"
    "documentation-comments.md"
    "bibliography.md"
    )

# unpack the package to ./smarten
tar -xvf ../.github/workflows/dependencies/EcmaTC49.Smarten.tar

for file in "${SPEC_FILES[@]}"
do
    echo "$file"
    ./smarten/Smarten_CL.exe $SPEC_DIRECTORY/$file $SPEC_DIRECTORY/$file
done

# finally, remove the unpacked smarten executable:
rm -rf smarten

# I think always success, but echo the success output anyway:
echo "::set-output name=status::success" 
