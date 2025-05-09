#!/bin/bash
set -eu -o pipefail


declare -a SPEC_FILES=($(ls ../standard/*.md))

# unpack the package to ./smarten
tar -xvf ../.github/workflows/dependencies/EcmaTC49.Smarten.tar

for file in "${SPEC_FILES[@]}"
do
    echo "$file"
    ./smarten/Smarten_CL.exe $file $file
done

# finally, remove the unpacked smarten executable:
rm -rf smarten

# I think always success, but echo the success output anyway:
if [ -n "$GITHUB_OUTPUT" ]
then
    echo "status=success" >> $GITHUB_OUTPUT
fi
