echo off
del grammar.md
echo Insert General/Lexical Headers
type grammar-general-lexical-insert.md >grammar.md
echo Extract Lexical Grammar
ExtractGrammar <md\lexical-structure.md >>grammar.md
echo Insert Syntactic Header
type grammar-syntactic-insert.md >>grammar.md 
echo Extract Syntactic Grammar
ExtractGrammar <md\basic-concepts.md >>grammar.md
ExtractGrammar <md\types.md >>grammar.md
ExtractGrammar <md\variables.md >>grammar.md
ExtractGrammar <md\conversions.md >>grammar.md
ExtractGrammar <md\expressions.md >>grammar.md
ExtractGrammar <md\statements.md >>grammar.md
ExtractGrammar <md\namespaces.md >>grammar.md
ExtractGrammar <md\classes.md >>grammar.md
ExtractGrammar <md\structs.md >>grammar.md
ExtractGrammar <md\arrays.md >>grammar.md
ExtractGrammar <md\interfaces.md >>grammar.md
ExtractGrammar <md\enums.md >>grammar.md
ExtractGrammar <md\delegates.md >>grammar.md
ExtractGrammar <md\exceptions.md >>grammar.md
ExtractGrammar <md\attributes.md >>grammar.md
echo Insert Unsafe Header
type grammar-unsafe-extensions-insert.md >>grammar.md 
echo Extract Unsafe Grammar
ExtractGrammar <md\unsafe-code.md >>grammar.md
echo Insert EOF Stuff
type grammar-eof-insert.md >>grammar.md 
