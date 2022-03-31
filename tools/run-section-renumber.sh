#!/bin/bash

set -e

declare -r PROJECT=StandardAnchorTags

if [ "$1" == "--dryrun" ]
then
    echo "Performing a dry run"
fi

dotnet run --project $PROJECT -- $1

if [ "$?" -eq "0" ]
then
    # Success: Write key/value for GitHub action to read:
    echo "::set-output name=status::success" 
else
    # Failed: report the error to the GitHub action:
    echo "::set-output name=status::failed" 
fi
