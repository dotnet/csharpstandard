#!/bin/bash
set -o pipefail

# Grammar Testing 2.1.0
# cwd when called is <repo>/tools

# the tarball is in <repo>/.github/workflows/dependencies/GrammarTestingEnv.tgz
TarBall=$(realpath ../.github/workflows/dependencies/GrammarTestingEnv.tgz)

# the testing directory is <repo>/tools/GrammarTesting
GrammarTesting=$(realpath ./GrammarTesting)

# the Standard source is in ../standard
StandardSource=$(realpath ../standard)

# unpack the tarball
pushd "${GrammarTesting}" >& /dev/null
tar -xf "${TarBall}"
# all unpacked and cwd now $GrammarTesting

# The testing package comes without the antlr jar or executable versions of BuildGrammar & TextModify
# - users download the first and build the others from source
# For V1 this script downloaded the JAR and the grammar-validator.yaml installed BuildGrammar from
# a nupkg as a dotnet tool called "csharpgrammar". TextModify was not required.
# For V2:
#  - Download the JAR to where the testing package expects.
#  - Leave the YAML to install BuildGrammar as a dotnet tool as before
#  - Create a BuildGrammar script which invokes "dotnet csharpgrammar"
#  - Create a TextModify script which does nothing - it is not used but santity checking looks for it

# Download the JAR
curl -H "Accept: application/zip" --no-progress-meter https://repo1.maven.org/maven2/org/antlr/antlr4/4.9.2/antlr4-4.9.2-complete.jar -o Environment/Antlr/antlr-4.9.2-complete.jar

# Move to the bin folder and create the two shell scripts
pushd Environment/Tools/bin >& /dev/null

cat >BuildGrammar <<EOF
#!/bin/bash
dotnet csharpgrammar "\$@"
EOF
chmod +x BuildGrammar
cat >TextModify <<EOF
#!/bin/bash
echo TextModify is not installed
EOF
chmod +x TextModify

popd >& /dev/null

# We should now be able to run the testing package scripts...
./SetupAndTest "${StandardSource}"

