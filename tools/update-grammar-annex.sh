#!/bin/bash
set -eu -o pipefail

declare -r GRAMMAR_PROJECT=GetGrammar
declare -r SPEC_DIRECTORY=../standard
declare -r OUTPUT_FILE=../standard/grammar.md

# Note that lexical structure and unsafe code are not in the array
# There are headers inserted before them.
declare -a SPEC_FILES=(
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
    )

dotnet build $GRAMMAR_PROJECT -c Release
dotnet publish $GRAMMAR_PROJECT -c Release -o $GRAMMAR_PROJECT/publish

echo Insert General/Lexical Headers
cat $GRAMMAR_PROJECT/grammar-general-lexical-insert.md >$OUTPUT_FILE
dotnet $GRAMMAR_PROJECT/publish/$GRAMMAR_PROJECT.dll $SPEC_DIRECTORY/lexical-structure.md >>$OUTPUT_FILE

echo Insert Syntactic Header
cat $GRAMMAR_PROJECT/grammar-syntactic-insert.md >>$OUTPUT_FILE

for file in "${SPEC_FILES[@]}"
do
   echo "$file"
   dotnet $GRAMMAR_PROJECT/publish/$GRAMMAR_PROJECT.dll $SPEC_DIRECTORY/$file >>$OUTPUT_FILE
done

echo Insert Unsafe Header
cat $GRAMMAR_PROJECT/grammar-unsafe-extensions-insert.md >>$OUTPUT_FILE
dotnet $GRAMMAR_PROJECT/publish/$GRAMMAR_PROJECT.dll $SPEC_DIRECTORY/unsafe-code.md >>$OUTPUT_FILE
echo Insert EOF Stuff
cat $GRAMMAR_PROJECT/grammar-eof-insert.md >>$OUTPUT_FILE

# I think always success, but echo the success output anyway:
echo "::set-output name=status::success" 
