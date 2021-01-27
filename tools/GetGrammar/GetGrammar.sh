#!/bin/sh
dotnet build -c Release
dotnet publish -o publish

rm grammar.g4
rm ../../standard/grammar.md
echo Insert General/Lexical Headers
cat grammar-general-lexical-insert.md >../../standard/grammar.md
echo Extract Lexical Grammar
dotnet publish/GetGrammar.dll ../../standard/lexical-structure.md >lexical-structure.g4
cat lexical-structure.g4 >grammar.g4
cat lexical-structure.g4 >>../../standard/grammar.md
rm lexical-structure.g4

echo Insert Syntactic Header
cat grammar-syntactic-insert.md >>../../standard/grammar.md 
echo Extract Syntactic Grammar

dotnet publish/GetGrammar.dll ../../standard/basic-concepts.md >basic-concepts.g4
cat basic-concepts.g4 >>grammar.g4
cat basic-concepts.g4 >>../../standard/grammar.md
rm basic-concepts.g4

dotnet publish/GetGrammar.dll ../../standard/types.md >types.g4
cat types.g4 >>grammar.g4
cat types.g4 >>../../standard/grammar.md
rm types.g4

dotnet publish/GetGrammar.dll ../../standard/variables.md >variables.g4
cat variables.g4 >>grammar.g4
cat variables.g4 >>../../standard/grammar.md
rm variables.g4

dotnet publish/GetGrammar.dll ../../standard/conversions.md >conversions.g4
cat conversions.g4 >>grammar.g4
cat conversions.g4 >>../../standard/grammar.md
rm conversions.g4

dotnet publish/GetGrammar.dll ../../standard/expressions.md >expressions.g4
cat expressions.g4 >>grammar.g4
cat expressions.g4 >>../../standard/grammar.md
rm expressions.g4

dotnet publish/GetGrammar.dll ../../standard/statements.md >statements.g4
cat statements.g4 >>grammar.g4
cat statements.g4 >>../../standard/grammar.md
rm statements.g4

dotnet publish/GetGrammar.dll ../../standard/namespaces.md >namespaces.g4
cat namespaces.g4 >>grammar.g4
cat namespaces.g4 >>../../standard/grammar.md
rm namespaces.g4

dotnet publish/GetGrammar.dll ../../standard/classes.md >classes.g4
cat classes.g4 >>grammar.g4
cat classes.g4 >>../../standard/grammar.md
rm classes.g4

dotnet publish/GetGrammar.dll ../../standard/structs.md >structs.g4
cat structs.g4 >>grammar.g4
cat structs.g4 >>../../standard/grammar.md
rm structs.g4

dotnet publish/GetGrammar.dll ../../standard/arrays.md >arrays.g4
cat arrays.g4 >>grammar.g4
cat arrays.g4 >>../../standard/grammar.md
rm arrays.g4

dotnet publish/GetGrammar.dll ../../standard/interfaces.md >interfaces.g4
cat interfaces.g4 >>grammar.g4
cat interfaces.g4 >>../../standard/grammar.md
rm interfaces.g4

dotnet publish/GetGrammar.dll ../../standard/enums.md >enums.g4
cat enums.g4 >>grammar.g4
cat enums.g4 >>../../standard/grammar.md
rm enums.g4

dotnet publish/GetGrammar.dll ../../standard/delegates.md >delegates.g4
cat delegates.g4 >>grammar.g4
cat delegates.g4 >>../../standard/grammar.md
rm delegates.g4

dotnet publish/GetGrammar.dll ../../standard/exceptions.md >exceptions.g4
cat exceptions.g4 >>grammar.g4
cat exceptions.g4 >>../../standard/grammar.md
rm exceptions.g4

dotnet publish/GetGrammar.dll ../../standard/attributes.md >attributes.g4
cat attributes.g4 >>grammar.g4
cat attributes.g4 >>../../standard/grammar.md
rm attributes.g4

echo Insert Unsafe Header
cat grammar-unsafe-extensions-insert.md >>../../standard/grammar.md 
echo Extract Unsafe Grammar
dotnet publish/GetGrammar.dll ../../standard/unsafe-code.md >unsafe-code.g4
cat unsafe-code.g4 >>grammar.g4
cat unsafe-code.g4 >>../../standard/grammar.md
rm unsafe-code.g4

echo Insert EOF Stuff
cat grammar-eof-insert.md >>../../standard/grammar.md 
