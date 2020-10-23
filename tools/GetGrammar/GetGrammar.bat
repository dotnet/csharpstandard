echo off
dotnet build
dotnet publish

del ..\..\standard\grammar.md
echo Insert General/Lexical Headers
type grammar-general-lexical-insert.md >..\..\standard\grammar.md
echo Extract Lexical Grammar
bin\Debug\netcoreapp3.1\publish\GetGrammar <..\..\standard\lexical-structure.md >>..\..\standard\grammar.md
echo Insert Syntactic Header
type grammar-syntactic-insert.md >>..\..\standard\grammar.md 
echo Extract Syntactic Grammar
bin\Debug\netcoreapp3.1\publish\GetGrammar <..\..\standard\basic-concepts.md >>..\..\standard\grammar.md
bin\Debug\netcoreapp3.1\publish\GetGrammar <..\..\standard\types.md >>..\..\standard\grammar.md
bin\Debug\netcoreapp3.1\publish\GetGrammar <..\..\standard\variables.md >>..\..\standard\grammar.md
bin\Debug\netcoreapp3.1\publish\GetGrammar <..\..\standard\conversions.md >>..\..\standard\grammar.md
bin\Debug\netcoreapp3.1\publish\GetGrammar <..\..\standard\expressions.md >>..\..\standard\grammar.md
bin\Debug\netcoreapp3.1\publish\GetGrammar <..\..\standard\statements.md >>..\..\standard\grammar.md
bin\Debug\netcoreapp3.1\publish\GetGrammar <..\..\standard\namespaces.md >>..\..\standard\grammar.md
bin\Debug\netcoreapp3.1\publish\GetGrammar <..\..\standard\classes.md >>..\..\standard\grammar.md
bin\Debug\netcoreapp3.1\publish\GetGrammar <..\..\standard\structs.md >>..\..\standard\grammar.md
bin\Debug\netcoreapp3.1\publish\GetGrammar <..\..\standard\arrays.md >>..\..\standard\grammar.md
bin\Debug\netcoreapp3.1\publish\GetGrammar <..\..\standard\interfaces.md >>..\..\standard\grammar.md
bin\Debug\netcoreapp3.1\publish\GetGrammar <..\..\standard\enums.md >>..\..\standard\grammar.md
bin\Debug\netcoreapp3.1\publish\GetGrammar <..\..\standard\delegates.md >>..\..\standard\grammar.md
bin\Debug\netcoreapp3.1\publish\GetGrammar <..\..\standard\exceptions.md >>..\..\standard\grammar.md
bin\Debug\netcoreapp3.1\publish\GetGrammar <..\..\standard\attributes.md >>..\..\standard\grammar.md
echo Insert Unsafe Header
type grammar-unsafe-extensions-insert.md >>..\..\standard\grammar.md 
echo Extract Unsafe Grammar
bin\Debug\netcoreapp3.1\publish\GetGrammar <..\..\standard\unsafe-code.md >>..\..\standard\grammar.md
echo Insert EOF Stuff
type grammar-eof-insert.md >>..\..\standard\grammar.md 
