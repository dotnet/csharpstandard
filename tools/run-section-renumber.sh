#!/bin/bash

set -e

declare -r PROJECT=StandardAnchorTags

dotnet run --project $PROJECT -- --owner dotnet --repo csharpstandard --dryrun true --head-sha $1

if [ -n "$GITHUB_OUTPUT" ]
then
    echo "status=success" >> $GITHUB_OUTPUT 
fi
