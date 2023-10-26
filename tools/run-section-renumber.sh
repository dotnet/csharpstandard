#!/bin/bash

set -e

declare -r PROJECT=StandardAnchorTags

if [ "$1" == "--dryrun" ]
then
    echo "Performing a dry run"
fi

dotnet run --project $PROJECT -- $1

if [ -n "$GITHUB_OUTPUT" ]
then
    if [ "$?" -eq "0" ]
    then
        # Success: Write key/value for GitHub action to read:
        echo "status=success" >> $GITHUB_OUTPUT 
    else
        # Failed: report the error to the GitHub action:
        echo "status=failed" >> $GITHUB_OUTPUT 
    fi
fi
