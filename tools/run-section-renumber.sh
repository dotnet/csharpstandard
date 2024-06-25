#!/bin/bash

set -e

declare -r PROJECT=StandardAnchorTags
declare -r STATUS_CHECK_PARMS=""

dotnet run --project $PROJECT -- --owner dotnet --repo csharpstandard

if [ -n "$GITHUB_OUTPUT" ]
then
    echo "status=success" >> $GITHUB_OUTPUT 
fi
