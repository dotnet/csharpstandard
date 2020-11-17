#!/bin/bash

set -e

# This is a convenient script to test the conversion from Markdown to Word.
# The script assumes that:
# - It's being run from the directory in which it's present
# - The spec Markdown is in ../standard
# - It can wipe and recreate a tmp directory
# - You want to use the default template (currently in the MarkdownConverter directory)

declare -r CONVERTER_PROJECT=MarkdownConverter
declare -r SPEC_DIRECTORY=../standard
declare -r TEMPLATE=$CONVERTER_PROJECT/template.docx
declare -r OUTPUT_DIRECTORY=tmp

rm -rf $OUTPUT_DIRECTORY
mkdir $OUTPUT_DIRECTORY

dotnet run -p $CONVERTER_PROJECT -- \
  $SPEC_DIRECTORY/*.md $TEMPLATE \
  -o $OUTPUT_DIRECTORY/standard.docx

