#!/bin/bash
set -eu -o pipefail

declare -r GRAMMAR_PROJECT=GetGrammar
declare -r SPEC_DIRECTORY=../standard
declare -r OUTPUT_FILE=csharp_grammar.g4

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

dotnet build $GRAMMAR_PROJECT -c Release
dotnet publish $GRAMMAR_PROJECT -c Release -o $GRAMMAR_PROJECT/publish

echo "grammar csharp_grammar;" > $OUTPUT_FILE
cat $GRAMMAR_PROJECT/grammar-lexer-members.txt >>$OUTPUT_FILE

for file in "${SPEC_FILES[@]}"
do
   echo "$file"
   dotnet $GRAMMAR_PROJECT/publish/$GRAMMAR_PROJECT.dll $SPEC_DIRECTORY/$file >>$OUTPUT_FILE
done

# Now, validate it:
curl -H "Accept: application/zip" https://repo1.maven.org/maven2/com/tunnelvisionlabs/antlr4/4.9.0/antlr4-4.9.0-complete.jar -o antlr-4.9.0-complete.jar
java -jar antlr-4.9.0-complete.jar $OUTPUT_FILE
