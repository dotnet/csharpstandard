#!/bin/bash

set -e

declare -r PROJECT=StandardAnchorTags

if [ "$1" == "--dryrun" ]
then
    echo "Performing a dry run"
fi

dotnet run --project $PROJECT -- $1

if [ -n "$GITHUB_OUTPUT" ] && [ "$?" -eq "0" ]
then
    echo "status=success" >> $GITHUB_OUTPUT 
fi
