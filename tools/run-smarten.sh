#!/bin/bash
set -eu -o pipefail

declare -r SPEC_DIRECTORY=../standard
declare -r EXTENSION=md

declare -a SPEC_FILES=(
    "foreword"
    "introduction"
    "scope"
    "normative-references"
    "terms-and-definitions"
    "general-description"
    "conformance"
    "lexical-structure"
    "basic-concepts" 
    "types"
    "variables"
    "conversions"
    "expressions"
    "statements"
    "namespaces"
    "classes"
    "structs"
    "arrays"
    "interfaces"
    "enums"
    "delegates"
    "exceptions"
    "attributes"
    "unsafe-code"
    "grammar"
    "portability-issues"
    "standard-library"
    "documentation-comments"
    "bibliography"
    )

# unpack the package to ./smarten
tar -xvf ../.github/workflows/dependencies/EcmaTC49.Smarten.tar

# On some files, smarten can't "round trip", so creates
# a new file of the form <filename>_(RoundTrip).md.
# The extra logic is to remvoe those artifacts so we don't add them to the PR.
for file in "${SPEC_FILES[@]}"
do
    echo "$file"
    ./smarten/Smarten_CL.exe $SPEC_DIRECTORY/$file.$EXTENSION $SPEC_DIRECTORY/$file.$EXTENSION
    # If $file_(RoundTrip).md was created, remove it:
    if [ -f $SPEC_DIRECTORY/$file"_(RoundTrip)."$EXTENSION ]; then
       rm $SPEC_DIRECTORY/$file"_(RoundTrip)."$EXTENSION
    fi
done

# finally, remove the unpacked smarten executable:
rm -rf smarten

# I think always success, but echo the success output anyway:
echo "::set-output name=status::success" 
