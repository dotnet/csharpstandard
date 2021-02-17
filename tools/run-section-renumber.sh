#!/bin/bash

echo $1

set -e

declare -r PROJECT=MarkdownConverter

dotnet run -p $PROJECT -- --dryrun
