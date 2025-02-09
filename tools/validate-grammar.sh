#!/bin/bash
set -eu -o pipefail

declare -r SPEC_DIRECTORY=../standard

dotnet csharpgrammar ../test-grammar CSharp $SPEC_DIRECTORY -m ../.github/workflows/dependencies/ReplaceAndAdd.md

# Now, validate it:
curl -H "Accept: application/zip" https://repo1.maven.org/maven2/com/tunnelvisionlabs/antlr4/4.9.0/antlr4-4.9.0-complete.jar -o antlr-4.9.0-complete.jar
java -jar antlr-4.9.0-complete.jar ../test-grammar/CSharpLexer.g4 ../test-grammar/CSharpParser.g4
