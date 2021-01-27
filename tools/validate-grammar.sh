#!/bin/sh
dotnet build GetGrammar -c Release
dotnet publish GetGrammar -c Release -o GetGrammar/publish

rm grammar.g4
echo Extract Lexical Grammar
dotnet GetGrammar/publish/GetGrammar.dll ../../standard/lexical-structure.md >grammar.g4

dotnet GetGrammar/publish/GetGrammar.dll ../../standard/basic-concepts.md >>grammar.g4
dotnet GetGrammar/publish/GetGrammar.dll ../../standard/types.md >>grammar.g4
dotnet GetGrammar/publish/GetGrammar.dll ../../standard/variables.md >>grammar.g4
dotnet GetGrammar/publish/GetGrammar.dll ../../standard/conversions.md >>grammar.g4
dotnet GetGrammar/publish/GetGrammar.dll ../../standard/expressions.md >>grammar.g4
dotnet GetGrammar/publish/GetGrammar.dll ../../standard/statements.md >>grammar.g4
dotnet GetGrammar/publish/GetGrammar.dll ../../standard/namespaces.md >>grammar.g4
dotnet GetGrammar/publish/GetGrammar.dll ../../standard/classes.md >>grammar.g4
dotnet GetGrammar/publish/GetGrammar.dll ../../standard/structs.md >>grammar.g4
dotnet GetGrammar/publish/GetGrammar.dll ../../standard/arrays.md >>grammar.g4
dotnet GetGrammar/publish/GetGrammar.dll ../../standard/interfaces.md >>grammar.g4
dotnet GetGrammar/publish/GetGrammar.dll ../../standard/enums.md >>grammar.g4
dotnet GetGrammar/publish/GetGrammar.dll ../../standard/delegates.md >>grammar.g4
dotnet GetGrammar/publish/GetGrammar.dll ../../standard/exceptions.md >>grammar.g4
dotnet GetGrammar/publish/GetGrammar.dll ../../standard/attributes.md >>grammar.g4
dotnet GetGrammar/publish/GetGrammar.dll ../../standard/unsafe-code.md >>grammar.g4
