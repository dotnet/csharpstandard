echo off
dotnet build -c Release
dotnet publish -o publish

del grammar.antlr
del ..\..\standard\grammar.md
echo Insert General/Lexical Headers
type grammar-general-lexical-insert.md >..\..\standard\grammar.md
echo Extract Lexical Grammar
publish\GetGrammar <..\..\standard\lexical-structure.md >lexical-structure.antlr
type lexical-structure.antlr >grammar.antlr
type lexical-structure.antlr >>..\..\standard\grammar.md
del lexical-structure.antlr

echo Insert Syntactic Header
type grammar-syntactic-insert.md >>..\..\standard\grammar.md 
echo Extract Syntactic Grammar

publish\GetGrammar <..\..\standard\basic-concepts.md >basic-concepts.antlr
type basic-concepts.antlr >>grammar.antlr
type basic-concepts.antlr >>..\..\standard\grammar.md
del basic-concepts.antlr

publish\GetGrammar <..\..\standard\types.md >types.antlr
type types.antlr >>grammar.antlr
type types.antlr >>..\..\standard\grammar.md
del types.antlr

publish\GetGrammar <..\..\standard\variables.md >variables.antlr
type variables.antlr >>grammar.antlr
type variables.antlr >>..\..\standard\grammar.md
del variables.antlr

publish\GetGrammar <..\..\standard\conversions.md >conversions.antlr
type conversions.antlr >>grammar.antlr
type conversions.antlr >>..\..\standard\grammar.md
del conversions.antlr

publish\GetGrammar <..\..\standard\expressions.md >expressions.antlr
type expressions.antlr >>grammar.antlr
type expressions.antlr >>..\..\standard\grammar.md
del expressions.antlr

publish\GetGrammar <..\..\standard\statements.md >statements.antlr
type statements.antlr >>grammar.antlr
type statements.antlr >>..\..\standard\grammar.md
del statements.antlr

publish\GetGrammar <..\..\standard\namespaces.md >namespaces.antlr
type namespaces.antlr >>grammar.antlr
type namespaces.antlr >>..\..\standard\grammar.md
del namespaces.antlr

publish\GetGrammar <..\..\standard\classes.md >classes.antlr
type classes.antlr >>grammar.antlr
type classes.antlr >>..\..\standard\grammar.md
del classes.antlr

publish\GetGrammar <..\..\standard\structs.md >structs.antlr
type structs.antlr >>grammar.antlr
type structs.antlr >>..\..\standard\grammar.md
del structs.antlr

publish\GetGrammar <..\..\standard\arrays.md >arrays.antlr
type arrays.antlr >>grammar.antlr
type arrays.antlr >>..\..\standard\grammar.md
del arrays.antlr

publish\GetGrammar <..\..\standard\interfaces.md >interfaces.antlr
type interfaces.antlr >>grammar.antlr
type interfaces.antlr >>..\..\standard\grammar.md
del interfaces.antlr

publish\GetGrammar <..\..\standard\enums.md >enums.antlr
type enums.antlr >>grammar.antlr
type enums.antlr >>..\..\standard\grammar.md
del enums.antlr

publish\GetGrammar <..\..\standard\delegates.md >delegates.antlr
type delegates.antlr >>grammar.antlr
type delegates.antlr >>..\..\standard\grammar.md
del delegates.antlr

publish\GetGrammar <..\..\standard\exceptions.md >exceptions.antlr
type exceptions.antlr >>grammar.antlr
type exceptions.antlr >>..\..\standard\grammar.md
del exceptions.antlr

publish\GetGrammar <..\..\standard\attributes.md >attributes.antlr
type attributes.antlr >>grammar.antlr
type attributes.antlr >>..\..\standard\grammar.md
del attributes.antlr

echo Insert Unsafe Header
type grammar-unsafe-extensions-insert.md >>..\..\standard\grammar.md 
echo Extract Unsafe Grammar
publish\GetGrammar <..\..\standard\unsafe-code.md >unsafe-code.antlr
type unsafe-code.antlr >>grammar.antlr
type unsafe-code.antlr >>..\..\standard\grammar.md
del unsafe-code.antlr

echo Insert EOF Stuff
type grammar-eof-insert.md >>..\..\standard\grammar.md 
