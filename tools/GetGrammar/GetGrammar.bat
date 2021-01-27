echo off
dotnet build -c Release
dotnet publish -o publish

del grammar.g4
del ..\..\standard\grammar.md
echo Insert General/Lexical Headers
type grammar-general-lexical-insert.md >..\..\standard\grammar.md
echo Extract Lexical Grammar
publish\GetGrammar ..\..\standard\lexical-structure.md >lexical-structure.g4
type lexical-structure.g4 >grammar.g4
type lexical-structure.g4 >>..\..\standard\grammar.md
del lexical-structure.g4

echo Insert Syntactic Header
type grammar-syntactic-insert.md >>..\..\standard\grammar.md 
echo Extract Syntactic Grammar

publish\GetGrammar ..\..\standard\basic-concepts.md >basic-concepts.g4
type basic-concepts.g4 >>grammar.g4
type basic-concepts.g4 >>..\..\standard\grammar.md
del basic-concepts.g4

publish\GetGrammar ..\..\standard\types.md >types.g4
type types.g4 >>grammar.g4
type types.g4 >>..\..\standard\grammar.md
del types.g4

publish\GetGrammar ..\..\standard\variables.md >variables.g4
type variables.g4 >>grammar.g4
type variables.g4 >>..\..\standard\grammar.md
del variables.g4

publish\GetGrammar ..\..\standard\conversions.md >conversions.g4
type conversions.g4 >>grammar.g4
type conversions.g4 >>..\..\standard\grammar.md
del conversions.g4

publish\GetGrammar ..\..\standard\expressions.md >expressions.g4
type expressions.g4 >>grammar.g4
type expressions.g4 >>..\..\standard\grammar.md
del expressions.g4

publish\GetGrammar ..\..\standard\statements.md >statements.g4
type statements.g4 >>grammar.g4
type statements.g4 >>..\..\standard\grammar.md
del statements.g4

publish\GetGrammar ..\..\standard\namespaces.md >namespaces.g4
type namespaces.g4 >>grammar.g4
type namespaces.g4 >>..\..\standard\grammar.md
del namespaces.g4

publish\GetGrammar ..\..\standard\classes.md >classes.g4
type classes.g4 >>grammar.g4
type classes.g4 >>..\..\standard\grammar.md
del classes.g4

publish\GetGrammar ..\..\standard\structs.md >structs.g4
type structs.g4 >>grammar.g4
type structs.g4 >>..\..\standard\grammar.md
del structs.g4

publish\GetGrammar ..\..\standard\arrays.md >arrays.g4
type arrays.g4 >>grammar.g4
type arrays.g4 >>..\..\standard\grammar.md
del arrays.g4

publish\GetGrammar ..\..\standard\interfaces.md >interfaces.g4
type interfaces.g4 >>grammar.g4
type interfaces.g4 >>..\..\standard\grammar.md
del interfaces.g4

publish\GetGrammar ..\..\standard\enums.md >enums.g4
type enums.g4 >>grammar.g4
type enums.g4 >>..\..\standard\grammar.md
del enums.g4

publish\GetGrammar ..\..\standard\delegates.md >delegates.g4
type delegates.g4 >>grammar.g4
type delegates.g4 >>..\..\standard\grammar.md
del delegates.g4

publish\GetGrammar ..\..\standard\exceptions.md >exceptions.g4
type exceptions.g4 >>grammar.g4
type exceptions.g4 >>..\..\standard\grammar.md
del exceptions.g4

publish\GetGrammar ..\..\standard\attributes.md >attributes.g4
type attributes.g4 >>grammar.g4
type attributes.g4 >>..\..\standard\grammar.md
del attributes.g4

echo Insert Unsafe Header
type grammar-unsafe-extensions-insert.md >>..\..\standard\grammar.md 
echo Extract Unsafe Grammar
publish\GetGrammar ..\..\standard\unsafe-code.md >unsafe-code.g4
type unsafe-code.g4 >>grammar.g4
type unsafe-code.g4 >>..\..\standard\grammar.md
del unsafe-code.g4

echo Insert EOF Stuff
type grammar-eof-insert.md >>..\..\standard\grammar.md 
