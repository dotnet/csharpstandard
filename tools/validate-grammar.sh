#!/bin/bash
set -eu -o pipefail

declare -r SPEC_DIRECTORY=../standard

declare -a SPEC_FILES=(
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
    )

dotnet csharpgrammar ../test-grammar CSharp ../standard -m ../.github/workflows/dependencies/ReplaceAndAdd.md

# Now, validate it:
curl -H "Accept: application/zip" https://repo1.maven.org/maven2/com/tunnelvisionlabs/antlr4/4.9.0/antlr4-4.9.0-complete.jar -o antlr-4.9.0-complete.jar
java -jar antlr-4.9.0-complete.jar ../test-grammar/CSharpLexer.g4 ../test-grammar/CSharpParser.g4
