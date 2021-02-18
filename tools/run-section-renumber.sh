#!/bin/bash

set -e

declare -r PROJECT=StandardAnchorTags

if [[ "$1" == "--dryrun" ]] then
    echo "Performing a dry run"
fi

dotnet run -p $PROJECT -- $1
