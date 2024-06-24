#!/bin/bash

set -e

declare -r PROJECT=StandardAnchorTags
declare -r STATUS_CHECK_PARMS=""

if [ -z "$1" ]
then
    STATUS_CHECK_PARMS="--dry-run --head-sha $1"
fi

dotnet run --project $PROJECT -- --owner dotnet --repo csharpstandard $STATUS_CHECK_PARMS

if [ -n "$GITHUB_OUTPUT" ]
then
    echo "status=success" >> $GITHUB_OUTPUT 
fi
