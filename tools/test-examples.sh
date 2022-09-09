#!/bin/bash

set -e

dotnet run --project ExampleExtractor -- ../standard example-templates tmp

echo ""

dotnet run --project ExampleTester -- tmp
