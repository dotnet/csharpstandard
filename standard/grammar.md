# Annex A Grammar

**This clause is informative.**

## A.1 General

This annex contains the grammar productions found in the specification, including the optional ones for unsafe code. Productions appear here in the same order in which they appear in the specification.

## A.2 Lexical grammar

```ANTLR

// Source: §7.3.1 General
Input
    : Input_Section?
    ;

Input_Section
    : Input_Section_Part+
    ;

Input_Section_Part
    : Input_Element* New_Line
    | Pp_Directive
    ;

Input_Element
    : Whitespace
    | Comment
    | Token
    ;

// Source: §7.3.2 Line terminators
  New_Line
    : '<Carriage return character (U+000D)>'
    | '<Line feed character (U+000A)>'
    | '<Carriage return character (U+000D) followed by line feed character (U+000A)>'
    | '<Next line character (U+0085)>'
    | '<Line separator character (U+2028)>'
    | '<Paragraph separator character (U+2029)>'
    ;

// Source: §7.3.3 Comments
Comment
    : Single_Line_Comment
    | Delimited_Comment
    ;

Single_Line_Comment
    : '//' Input_Character*
    ;

Input_Character
    : '<Any Unicode character except a New_Line_Character>'
    ;
    
New_Line_Character
    : '<Carriage return character (U+000D)>'
    | '<Line feed character (U+000A)>'
    | '<Next line character (U+0085)>'
    | '<Line separator character (U+2028)>'
    | '<Paragraph separator character (U+2029)>'
    ;
    
Delimited_Comment
    : '/*' Delimited_Comment_Section* Asterisk+ '/'
    ;
    
Delimited_Comment_Section
    : '/'
    | Asterisk* Not_Slash_Or_Asterisk
    ;

Asterisk
    : '*'
    ;

Not_Slash_Or_Asterisk
    : '<Any Unicode character except / or *>'
    ;

// Source: §7.3.4 White space
Whitespace
    : '<Any character with Unicode class Zs>'
    | '<Horizontal tab character (U+0009)>'
    | '<Vertical tab character (U+000B)>'
    | '<Form feed character (U+000C)>'
    ;

// Source: §7.4.1 General
Token
    : Identifier
    | Keyword
    | Integer_Literal
    | Real_Literal
    | Character_Literal
    | String_Literal
    | Operator_Or_Punctuator
    ;

// Source: §7.4.2 Unicode character escape sequences
Unicode_Escape_Sequence
    : '\\u' Hex_Digit Hex_Digit Hex_Digit Hex_Digit
    | '\\U' Hex_Digit Hex_Digit Hex_Digit Hex_Digit Hex_Digit Hex_Digit Hex_Digit Hex_Digit
    ;

// Source: §7.4.3 Identifiers
Identifier
    : Available_Identifier
    | '@' Identifier_Or_Keyword
    ;

Available_Identifier
    : '<An Identifier_Or_Keyword that is not a Keyword>'
    ;

Identifier_Or_Keyword
    : Identifier_Start_Character Identifier_Part_Character*
    ;

Identifier_Start_Character
    : Letter_Character
    | Underscore_Character
    ;

Underscore_Character
    : '<_ the underscore character (U+005F)>'
    | '<A Unicode_Escape_Sequence representing the character U+005F>'
    ;

Identifier_Part_Character
    : Letter_Character
    | Decimal_Digit_Character
    | Connecting_Character
    | Combining_Character
    | Formatting_Character
    ;

Letter_Character
    : '<A Unicode character of classes Lu, Ll, Lt, Lm, Lo, or Nl>'
    | '<A Unicode_Escape_Sequence representing a character of classes Lu, Ll, Lt, Lm, Lo, or Nl>'
    ;

Combining_Character
    : '<A Unicode character of classes Mn or Mc>'
    | '<A Unicode_Escape_Sequence representing a character of classes Mn or Mc>'
    ;

Decimal_Digit_Character
    : '<A Unicode character of the class Nd>'
    | '<A Unicode_Escape_Sequence representing a character of the class Nd>'
    ;

Connecting_Character
    : '<A Unicode character of the class Pc>'
    | '<A Unicode_Escape_Sequence representing a character of the class Pc>'
    ;

Formatting_Character
    : '<A Unicode character of the class Cf>'
    | '<A Unicode_Escape_Sequence representing a character of the class Cf>'
    ;

// Source: §7.4.4 Keywords
Keyword
    : 'abstract' | 'as'       | 'base'       | 'bool'      | 'break'
    | 'byte'     | 'case'     | 'catch'      | 'char'      | 'checked'
    | 'class'    | 'const'    | 'continue'   | 'decimal'   | 'default'
    | 'delegate' | 'do'       | 'double'     | 'else'      | 'enum'
    | 'event'    | 'explicit' | 'extern'     | 'false'     | 'finally'
    | 'fixed'    | 'float'    | 'for'        | 'foreach'   | 'goto'
    | 'if'       | 'implicit' | 'in'         | 'int'       | 'interface'
    | 'internal' | 'is'       | 'lock'       | 'long'      | 'namespace'
    | 'new'      | 'null'     | 'object'     | 'operator'  | 'out'
    | 'override' | 'params'   | 'private'    | 'protected' | 'public'
    | 'readonly' | 'ref'      | 'return'     | 'sbyte'     | 'sealed'
    | 'short'    | 'sizeof'   | 'stackalloc' | 'static'    | 'string'
    | 'struct'   | 'switch'   | 'this'       | 'throw'     | 'true'
    | 'try'      | 'typeof'   | 'uint'       | 'ulong'     | 'unchecked'
    | 'unsafe'   | 'ushort'   | 'using'      | 'virtual'   | 'void'
    | 'volatile' | 'while'
    ;

// Source: §7.4.4 Keywords
Contextual_Keyword
    : 'add'    'alias'         'ascending'   'async'      'await'
    | 'by'     'descending'    'dynamic'     'equals'     'from'
    | 'get'    'global'        'group'       'into'       'join'
    | 'let'    'orderby'       'partial'     'remove'     'select'
    | 'set'    'value'         'var'         'where'      'yield'
    ;

// Source: §7.4.5.1 General
Literal
    : Boolean_Literal
    | Integer_Literal
    | Real_Literal
    | Character_Literal
    | String_Literal
    | Null_Literal
    ;

// Source: §7.4.5.2 Boolean literals
Boolean_Literal
    : 'true'
    | 'false'
    ;

// Source: §7.4.5.3 Integer literals
Integer_Literal
    : Decimal_Integer_Literal
    | Hexadecimal_Integer_Literal
    ;

Decimal_Integer_Literal
    : Decimal_Digit+ Integer_Type_Suffix?
    ;
    
Decimal_Digit
    : '0' | '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9'
    ;
    
Integer_Type_Suffix
    : 'U' | 'u' | 'L' | 'l' | 'UL' | 'Ul' | 'uL' | 'ul' | 'LU' | 'Lu' | 'lU' | 'lu'
    ;
    
Hexadecimal_Integer_Literal
    : '0x' Hex_Digit+ Integer_Type_Suffix?
    | '0X' Hex_Digit+ Integer_Type_Suffix?
    ;

Hex_Digit
    : '0' | '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9'
    | 'A' | 'B' | 'C' | 'D' | 'E' | 'F' | 'a' | 'b' | 'c' | 'd' | 'e' | 'f';

// Source: §7.4.5.4 Real literals
Real_Literal
    : Decimal_Digit+ '.' Decimal_Digit+ Exponent_Part? Real_Type_Suffix?
    | '.' Decimal_Digit+ Exponent_Part? Real_Type_Suffix?
    | Decimal_Digit+ Exponent_Part Real_Type_Suffix?
    | Decimal_Digit+ Real_Type_Suffix
    ;

Exponent_Part
    : 'e' Sign? Decimal_Digit+
    | 'E' Sign? Decimal_Digit+
    ;

Sign
    : '+' | '-'
    ;

Real_Type_Suffix
    : 'F' | 'f' | 'D' | 'd' | 'M' | 'm'
    ;

// Source: §7.4.5.5 Character literals
Character_Literal
    : '\'' Character '\''
    ;
    
Character
    : Single_Character
    | Simple_Escape_Sequence
    | Hexadecimal_Escape_Sequence
    | Unicode_Escape_Sequence
    ;
    
Single_Character
    : '<Any character except \' (U+0027), \\ (U+005C), and New_Line_Character>'
    ;
    
Simple_Escape_Sequence
    : '\\\'' | '\\"' | '\\\\' | '\\0' | '\\a' | '\\b' | '\\f' | '\\n' | '\\r' | '\\t' | '\\v'
    ;
    
Hexadecimal_Escape_Sequence
    : '\\x' Hex_Digit Hex_Digit? Hex_Digit? Hex_Digit?
    ;

// Source: §7.4.5.6 String literals
String_Literal
    : Regular_String_Literal
    | Verbatim_String_Literal
    ;
    
Regular_String_Literal
    : '"' Regular_String_Literal_Character* '"'
    ;
    
Regular_String_Literal_Character
    : Single_Regular_String_Literal_Character
    | Simple_Escape_Sequence
    | Hexadecimal_Escape_Sequence
    | Unicode_Escape_Sequence
    ;

Single_Regular_String_Literal_Character
    : '<Any character except \" (U+0022), \\ (U+005C), and New_Line_Character>'
    ;

Verbatim_String_Literal
    : '@"' Verbatim_String_Literal_Character* '"'
    ;
    
Verbatim_String_Literal_Character
    : Single_Verbatim_String_Literal_Character
    | Quote_Escape_Sequence
    ;
    
Single_Verbatim_String_Literal_Character
    : '<any character except ">'
    ;
    
Quote_Escape_Sequence
    : '""'
    ;

// Source: §7.4.5.7 The null literal
Null_Literal
    : 'null'
    ;

// Source: §7.4.6 Operators and punctuators
Operator_Or_Punctuator
    : '{'  | '}'  | '['  | ']'  | '('   | ')'  | '.'  | ','  | ':'  | ';'
    | '+'  | '-'  | '*'  | '/'  | '%'   | '&'  | '|'  | '^'  | '!'  | '~'
    | '='  | '<'  | '>'  | '?'  | '??'  | '::' | '++' | '--' | '&&' | '||'
    | '->' | '==' | '!=' | '<=' | '>='  | '+=' | '-=' | '*=' | '/=' | '%='
    | '&=' | '|=' | '^=' | '<<' | '<<=' | '=>'
    ;
    
Right_Shift
    : '>'  '>'
    ;

Right_Shift_Assignment
    : '>' '>='
    ;

// Source: §7.5.1 General
Pp_Directive
    : Pp_Declaration
    | Pp_Conditional
    | Pp_Line
    | Pp_Diagnostic
    | Pp_Region
    | Pp_Pragma
    ;

// Source: §7.5.2 Conditional compilation symbols
Conditional_Symbol
    : '<Any Identifier_Or_Keyword except true or false>'
    ;

// Source: §7.5.3 Pre-processing expressions
Pp_Expression
    : Whitespace? Pp_Or_Expression Whitespace?
    ;
    
Pp_Or_Expression
    : Pp_And_Expression
    | Pp_Or_Expression Whitespace? '||' Whitespace? Pp_And_Expression
    ;
    
Pp_And_Expression
    : Pp_Equality_Expression
    | Pp_And_Expression Whitespace? '&&' Whitespace? Pp_Equality_Expression
    ;

Pp_Equality_Expression
    : Pp_Unary_Expression
    | Pp_Equality_Expression Whitespace? '==' Whitespace? Pp_Unary_Expression
    | Pp_Equality_Expression Whitespace? '!=' Whitespace? Pp_Unary_Expression
    ;
    
Pp_Unary_Expression
    : Pp_Primary_Expression
    | '!' Whitespace? Pp_Unary_Expression
    ;
    
Pp_Primary_Expression
    : 'true'
    | 'false'
    | Conditional_Symbol
    | '(' Whitespace? Pp_Expression Whitespace? ')'
    ;

// Source: §7.5.4 Definition directives
Pp_Declaration
    : Whitespace? '#' Whitespace? 'define' Whitespace Conditional_Symbol Pp_New_Line
    | Whitespace? '#' Whitespace? 'undef' Whitespace Conditional_Symbol Pp_New_Line
    ;

Pp_New_Line
    : Whitespace? Single_Line_Comment? New_Line
    ;

// Source: §7.5.5 Conditional compilation directives
Pp_Conditional
    : Pp_If_Section Pp_Elif_Section* Pp_Else_Section? Pp_Endif
    ;

Pp_If_Section
    : Whitespace? '#' Whitespace? 'if' Whitespace Pp_Expression Pp_New_Line Conditional_Section?
    ;
    
Pp_Elif_Section
    : Whitespace? '#' Whitespace? 'elif' Whitespace Pp_Expression Pp_New_Line Conditional_Section?
    ;
    
Pp_Else_Section
    : Whitespace? '#' Whitespace? 'else' Pp_New_Line Conditional_Section?
    ;
    
Pp_Endif
    : Whitespace? '#' Whitespace? 'endif' Pp_New_Line
    ;
    
Conditional_Section
    : Input_Section
    | Skipped_Section_Part+
    ;

Skipped_Section_Part
    : Skipped_Characters? New_Line
    | Pp_Directive
    ;
    
Skipped_Characters
    : Whitespace? Not_Number_Sign Input_Character*
    ;

Not_Number_Sign
    : '<Any Input_Character except #>'
    ;

// Source: §7.5.6 Diagnostic directives
Pp_Diagnostic
    : Whitespace? '#' Whitespace? 'error' Pp_Message
    | Whitespace? '#' Whitespace? 'warning' Pp_Message
    ;

Pp_Message
    : New_Line
    | Whitespace Input_Character* New_Line
    ;

// Source: §7.5.7 Region directives
Pp_Region
    : Pp_Start_Region Conditional_Section? Pp_End_Region
    ;

Pp_Start_Region
    : Whitespace? '#' Whitespace? 'region' Pp_Message
    ;

Pp_End_Region
    : Whitespace? '#' Whitespace? 'endregion' Pp_Message
    ;

// Source: §7.5.8 Line directives
Pp_Line
    : Whitespace? '#' Whitespace? 'line' Whitespace Line_Indicator Pp_New_Line
    ;

Line_Indicator
    : Decimal_Digit+ Whitespace Compilation_Unit_Name
    | Decimal_Digit+
    | 'default'
    | 'hidden'
    ;
    
Compilation_Unit_Name
    : '"' Compilation_Unit_Name_Character+ '"'
    ;
    
Compilation_Unit_Name_Character
    : '<Any input_character except " (U+0022), and New_Line_Character>'
    ;

// Source: §7.5.9 Pragma directives
Pp_Pragma
    : Whitespace? '#' Whitespace? 'pragma' Pp_Pragma_Text
    ;

Pp_Pragma_Text
    : New_Line
    | Whitespace Input_Character* New_Line
    ;
```

## A.3 Syntactic grammar

```ANTLR

// Source: §8.8.1 General
namespace_name
    : namespace_or_type_name
    ;

type_name
    : namespace_or_type_name
    ;
    
namespace_or_type_name
    : Identifier type_argument_list?
    | namespace_or_type_name '.' Identifier type_argument_list?
    | qualified_alias_member
    ;

// Source: §9.1 General
type
    : reference_type
    | value_type
    | type_parameter
    ;

// Source: §9.2.1 General
reference_type
    : class_type
    | interface_type
    | array_type
    | delegate_type
    | 'dynamic'
    ;

class_type
    : type_name
    | 'object'
    | 'string'
    ;

interface_type
    : type_name
    ;

array_type
    : non_array_type rank_specifier+
    ;

non_array_type
    : value_type
    | class_type
    | interface_type
    | delegate_type
    | 'dynamic'
    | type_parameter
    ;

rank_specifier
    : '[' ','* ']'
    ;

delegate_type
    : type_name
    ;

// Source: §9.3.1 General
value_type
    : struct_type
    | enum_type
    ;

struct_type
    : type_name
    | simple_type
    | nullable_value_type
    ;

simple_type
    : numeric_type
    | 'bool'
    ;

numeric_type
    : integral_type
    | floating_point_type
    | 'decimal'
    ;

integral_type
    : 'sbyte'
    | 'byte'
    | 'short'
    | 'ushort'
    | 'int'
    | 'uint'
    | 'long'
    | 'ulong'
    | 'char'
    ;

floating_point_type
    : 'float'
    | 'double'
    ;

nullable_value_type
    : non_nullable_value_type '?'
    ;

non_nullable_value_type
    : type
    ;

enum_type
    : type_name
    ;

// Source: §9.4.2 Type arguments
type_argument_list
    : '<' type_arguments '>'
    ;

type_arguments
    : type_argument (',' type_argument)*
    ;   

type_argument
    : type
    ;

// Source: §9.5 Type parameters
type_parameter
    : Identifier
    ;

// Source: §10.5 Variable references
variable_reference
    : expression
    ;

// Source: §12.6.2.1 General
argument_list
    : argument (',' argument)*
    ;

argument
    : argument_name? argument_value
    ;

argument_name
    : Identifier ':'
    ;

argument_value
    : expression
    | 'ref' variable_reference
    | 'out' variable_reference
    ;

// Source: §12.7.1 General
primary_expression
    : primary_no_array_creation_expression
    | array_creation_expression
    ;

primary_no_array_creation_expression
    : Literal
    | simple_name
    | parenthesized_expression
    | member_access
    | invocation_expression
    | element_access
    | this_access
    | base_access
    | post_increment_expression
    | post_decrement_expression
    | object_creation_expression
    | delegate_creation_expression
    | anonymous_object_creation_expression
    | typeof_expression
    | sizeof_expression
    | checked_expression
    | unchecked_expression
    | default_value_expression
    | anonymous_method_expression
    ;

// Source: §12.7.3.1 General
simple_name
    : Identifier type_argument_list?
    ;

// Source: §12.7.4 Parenthesized expressions
parenthesized_expression
    : '(' expression ')'
    ;

// Source: §12.7.5.1 General
member_access
    : primary_expression '.' Identifier type_argument_list?
    | predefined_type '.' Identifier type_argument_list?
    | qualified_alias_member '.' Identifier type_argument_list?
    ;

predefined_type
    : 'bool'   | 'byte'  | 'char'  | 'decimal' | 'double' | 'float' | 'int' | 'long'
    | 'object' | 'sbyte' | 'short' | 'string'  | 'uint'   | 'ulong' | 'ushort'
    ;

// Source: §12.7.6.1 General
invocation_expression
    : primary_expression '(' argument_list? ')'
    ;

// Source: §12.7.7.1 General
element_access
    : primary_no_array_creation_expression '[' argument_list ']'
    ;

// Source: §12.7.8 This access
this_access
    : 'this'
    ;

// Source: §12.7.9 Base access
base_access
    : 'base' '.' identifier type_argument_list?
    | 'base' '[' argument_list ']'
    ;

// Source: §12.7.10 Postfix increment and decrement operators
post_increment_expression
    : primary_expression '++'
    ;

post_decrement_expression
    : primary_expression '--'
    ;

// Source: §12.7.11.2 Object creation expressions
object_creation_expression
    : 'new' type '(' argument_list? ')' object_or_collection_initializer?
    | 'new' type object_or_collection_initializer
    ;

object_or_collection_initializer
    : object_initializer
    | collection_initializer
    ;

// Source: §12.7.11.3 Object initializers
object_initializer
    : '{' member_initializer_list? '}'
    | '{' member_initializer_list ',' '}'
    ;

member_initializer_list
    : member_initializer (',' member_initializer)*
    ;

member_initializer
    : Identifier '=' initializer_value
    ;

initializer_value
    : expression
    | object_or_collection_initializer
    ;

// Source: §12.7.11.4 Collection initializers
collection_initializer
    : '{' element_initializer_list '}'
    | '{' element_initializer_list ',' '}'
    ;

element_initializer_list
    : element_initializer (',' element_initializer)*
    ;

element_initializer
    : non_assignment_expression
    | '{' expression_list '}'
    ;

expression_list
    : expression
    | expression_list ',' expression
    ;

// Source: §12.7.11.5 Array creation expressions
array_creation_expression
    : 'new' non_array_type '[' expression_list ']' rank_specifier* array_initializer?
    | 'new' array_type array_initializer
    | 'new' rank_specifier array_initializer
    ;

// Source: §12.7.11.6 Delegate creation expressions
delegate_creation_expression
    : 'new' delegate_type '(' expression ')'
    ;

// Source: §12.7.11.7 Anonymous object creation expressions
anonymous_object_creation_expression
    : 'new' anonymous_object_initializer
    ;

anonymous_object_initializer
    : '{' member_declarator_list? '}'
    | '{' member_declarator_list ',' '}'
    ;

member_declarator_list
    : member_declarator (',' member_declarator)*
    ;

member_declarator
    : simple_name
    | member_access
    | base_access
    | Identifier '=' expression
    ;

// Source: §12.7.12 The typeof operator
typeof_expression
    : 'typeof' '(' type ')'
    | 'typeof' '(' unbound_type_name ')'
    | 'typeof' '(' 'void' ')'
    ;

unbound_type_name
    : Identifier generic_dimension_specifier?
    | Identifier '::' Identifier generic_dimension_specifier?
    | unbound_type_name '.' Identifier generic_dimension_specifier?
    ;

generic_dimension_specifier
    : '<' comma* '>'
    ;

comma
    : ','
    ;


// Source: §12.7.13 The sizeof operator
sizeof_expression
   : 'sizeof' '(' unmanaged_type ')'
   ;

// Source: §12.7.14 The checked and unchecked operators
checked_expression
    : 'checked' '(' expression ')'
    ;

unchecked_expression
    : 'unchecked' '(' expression ')'
    ;

// Source: §12.7.15 Default value expressions
default_value_expression
    : 'default' '(' type ')'
    ;

// Source: §12.8.1 General
unary_expression
    : primary_expression
    | '+' unary_expression
    | '-' unary_expression
    | '!' unary_expression
    | '~' unary_expression
    | pre_increment_expression
    | pre_decrement_expression
    | cast_expression
    | await_expression
    ;

// Source: §12.8.6 Prefix increment and decrement operators
pre_increment_expression
    : '++' unary_expression
    ;

pre_decrement_expression
    : '--' unary_expression
    ;

// Source: §12.8.7 Cast expressions
cast_expression
    : '(' type ')' unary_expression
    ;

// Source: §12.8.8.1 General
await_expression
    : 'await' unary_expression
    ;

// Source: §12.9.1 General
multiplicative_expression
    : unary_expression
    | multiplicative_expression '*' unary_expression
    | multiplicative_expression '/' unary_expression
    | multiplicative_expression '%' unary_expression
    ;

additive_expression
    : multiplicative_expression
    | additive_expression '+' multiplicative_expression
    | additive_expression '-' multiplicative_expression
    ;

// Source: §12.10 Shift operators
shift_expression
    : additive_expression
    | shift_expression '<<' additive_expression
    | shift_expression Right_Shift additive_expression
    ;

// Source: §12.11.1 General
relational_expression
    : shift_expression
    | relational_expression '<' shift_expression
    | relational_expression '>' shift_expression
    | relational_expression '<=' shift_expression
    | relational_expression '>=' shift_expression
    | relational_expression 'is' type
    | relational_expression 'as' type
    ;

equality_expression
    : relational_expression
    | equality_expression '==' relational_expression
    | equality_expression '!=' relational_expression
    ;

// Source: §12.12.1 General
and_expression
    : equality_expression
    | and_expression '&' equality_expression
    ;

exclusive_or_expression
    : and_expression
    | exclusive_or_expression '^' and_expression
    ;

inclusive_or_expression
    : exclusive_or_expression
    | inclusive_or_expression '|' exclusive_or_expression
    ;

// Source: §12.13.1 General
conditional_and_expression
    : inclusive_or_expression
    | conditional_and_expression '&&' inclusive_or_expression
    ;

conditional_or_expression
    : conditional_and_expression
    | conditional_or_expression '||' conditional_and_expression
    ;

// Source: §12.14 The null coalescing operator
null_coalescing_expression
    : conditional_or_expression
    | conditional_or_expression '??' null_coalescing_expression
    ;

// Source: §12.15 Conditional operator
conditional_expression
    : null_coalescing_expression
    | null_coalescing_expression '?' expression ':' expression
    ;

// Source: §12.16.1 General
lambda_expression
    : 'async'? anonymous_function_signature '=>' anonymous_function_body
    ;

anonymous_method_expression
    : 'async'? 'delegate' explicit_anonymous_function_signature? block
    ;

anonymous_function_signature
    : explicit_anonymous_function_signature
    | implicit_anonymous_function_signature
    ;

explicit_anonymous_function_signature
    : '(' explicit_anonymous_function_parameter_list? ')'
    ;

explicit_anonymous_function_parameter_list
    : explicit_anonymous_function_parameter (',' explicit_anonymous_function_parameter)*
    ;

explicit_anonymous_function_parameter
    : anonymous_function_parameter_modifier? type Identifier
    ;

anonymous_function_parameter_modifier
    : 'ref'
    | 'out'
    ;

implicit_anonymous_function_signature
    : '(' implicit_anonymous_function_parameter_list? ')'
    | implicit_anonymous_function_parameter
    ;

implicit_anonymous_function_parameter_list
    : implicit_anonymous_function_parameter (',' implicit_anonymous_function_parameter)*
    ;

implicit_anonymous_function_parameter
    : Identifier
    ;

anonymous_function_body
    : expression
    | block
    ;

// Source: §12.17.1 General
query_expression
    : from_clause query_body
    ;

from_clause
    : 'from' type? Identifier 'in' expression
    ;

query_body
    : query_body_clauses? select_or_group_clause query_continuation?
    ;

query_body_clauses
    : query_body_clause
    | query_body_clauses query_body_clause
    ;

query_body_clause
    : from_clause
    | let_clause
    | where_clause
    | join_clause
    | join_into_clause
    | orderby_clause
    ;

let_clause
    : 'let' Identifier '=' expression
    ;

where_clause
    : 'where' boolean_expression
    ;

join_clause
    : 'join' type? Identifier 'in' expression 'on' expression 'equals' expression
    ;

join_into_clause
    : 'join' type? Identifier 'in' expression 'on' expression 'equals' expression 'into' Identifier
    ;

orderby_clause
    : 'orderby' orderings
    ;

orderings
    : ordering (',' ordering)*
    ;

ordering
    : expression ordering_direction?
    ;

ordering_direction
    : 'ascending'
    | 'descending'
    ;

select_or_group_clause
    : select_clause
    | group_clause
    ;

select_clause
    : 'select' expression
    ;

group_clause
    : 'group' expression 'by' expression
    ;

query_continuation
    : 'into' Identifier query_body
    ;

// Source: §12.18.1 General
assignment
    : unary_expression assignment_operator expression
    ;

assignment_operator
    : '=' | '+=' | '-=' | '*=' | '/=' | '%=' | '&=' | '|=' | '^=' | '<<='
    | Right_Shift_Assignment
    ;

// Source: §12.19 Expression
expression
    : non_assignment_expression
    | assignment
    ;

non_assignment_expression
    : conditional_expression
    | lambda_expression
    | query_expression
    ;

// Source: §12.20 Constant expressions
constant_expression
    : expression
    ;

// Source: §12.21 Boolean expressions
boolean_expression
    : expression
    ;

// Source: §13.1 General
statement
    : labeled_statement
    | declaration_statement
    | embedded_statement
    ;

embedded_statement
    : block
    | empty_statement
    | expression_statement
    | selection_statement
    | iteration_statement
    | jump_statement
    | try_statement
    | checked_statement
    | unchecked_statement
    | lock_statement
    | using_statement
    | yield_statement
    ;

// Source: §13.3.1 General
block
    : '{' statement_list? '}'
    ;

// Source: §13.3.2 Statement lists
statement_list
    : statement+
    ;

// Source: §13.4 The empty statement
empty_statement
    : ';'
    ;

// Source: §13.5 Labeled statements
labeled_statement
    : Identifier ':' statement
    ;

// Source: §13.6.1 General
declaration_statement
    : local_variable_declaration ';'
    | local_constant_declaration ';'
    ;

// Source: §13.6.2 Local variable declarations
local_variable_declaration
    : local_variable_type local_variable_declarators
    ;

local_variable_type
    : type
    | 'var'
    ;

local_variable_declarators
    : local_variable_declarator
    | local_variable_declarators ',' local_variable_declarator
    ;

local_variable_declarator
    : Identifier
    | Identifier '=' local_variable_initializer
    ;

local_variable_initializer
    : expression
    | array_initializer
    ;

// Source: §13.6.3 Local constant declarations
local_constant_declaration
    : 'const' type constant_declarators
    ;

constant_declarators
    : constant_declarator (',' constant_declarator)*
    ;

constant_declarator
    : Identifier '=' constant_expression
    ;

// Source: §13.7 Expression statements
expression_statement
    : statement_expression ';'
    ;

statement_expression
    : invocation_expression
    | object_creation_expression
    | assignment
    | post_increment_expression
    | post_decrement_expression
    | pre_increment_expression
    | pre_decrement_expression
    | await_expression
    ;

// Source: §13.8.1 General
selection_statement
    : if_statement
    | switch_statement
    ;

// Source: §13.8.2 The if statement
if_statement
    : 'if' '(' boolean_expression ')' embedded_statement
    | 'if' '(' boolean_expression ')' embedded_statement 'else' embedded_statement
    ;

// Source: §13.8.3 The switch statement
switch_statement
    : 'switch' '(' expression ')' switch_block
    ;

switch_block
    : '{' switch_section* '}'
    ;

switch_section
    : switch_label+ statement_list
    ;

switch_label
    : 'case' constant_expression ':'
    | 'default' ':'
    ;

// Source: §13.9.1 General
iteration_statement
    : while_statement
    | do_statement
    | for_statement
    | foreach_statement
    ;

// Source: §13.9.2 The while statement
while_statement
    : 'while' '(' boolean_expression ')' embedded_statement
    ;

// Source: §13.9.3 The do statement
do_statement
    : 'do' embedded_statement 'while' '(' boolean_expression ')' ';'
    ;

// Source: §13.9.4 The for statement
for_statement
    : 'for' '(' for_initializer? ';' for_condition? ';' for_iterator? ')' embedded_statement
    ;

for_initializer
    : local_variable_declaration
    | statement_expression_list
    ;

for_condition
    : boolean_expression
    ;

for_iterator
    : statement_expression_list
    ;

statement_expression_list
    : statement_expression (',' statement_expression)*
    ;

// Source: §13.9.5 The foreach statement
foreach_statement
    : 'foreach' '(' local_variable_type Identifier 'in' expression ')' embedded_statement
    ;

// Source: §13.10.1 General
jump_statement
    : break_statement
    | continue_statement
    | goto_statement
    | return_statement
    | throw_statement
    ;

// Source: §13.10.2 The break statement
break_statement
    : 'break' ';'
    ;

// Source: §13.10.3 The continue statement
continue_statement
    : 'continue' ';'
    ;

// Source: §13.10.4 The goto statement
goto_statement
    : 'goto' Identifier ';'
    | 'goto' 'case' constant_expression ';'
    | 'goto' 'default' ';'
    ;

// Source: §13.10.5 The return statement
return_statement
    : 'return' expression? ';'
    ;

// Source: §13.10.6 The throw statement
throw_statement
    : 'throw' expression? ';'
    ;

// Source: §13.11 The try statement
try_statement
    : 'try' block catch_clause+
    | 'try' block finally_clause
    | 'try' block catch_clause+ finally_clause
    ;

catch_clause
    :  'catch' exception_specifier?  block
    ;

exception_specifier
    : '(' type Identifier? ')'
    ;
    
finally_clause
    : 'finally' block
    ;

// Source: §13.12 The checked and unchecked statements
checked_statement
    : 'checked' block
    ;

unchecked_statement
    : 'unchecked' block
    ;

// Source: §13.13 The lock statement
lock_statement
    : 'lock' '(' expression ')' embedded_statement
    ;

// Source: §13.14 The using statement
using_statement
    : 'using' '(' resource_acquisition ')' embedded_statement
    ;

resource_acquisition
    : local_variable_declaration
    | expression
    ;

// Source: §13.15 The yield statement
yield_statement
    : 'yield' 'return' expression ';'
    | 'yield' 'break' ';'
    ;

// Source: §14.2 Compilation units
compilation_unit
    : extern_alias_directive* using_directive* global_attributes? namespace_member_declaration*
    ;

// Source: §14.3 Namespace declarations
namespace_declaration
    : 'namespace' qualified_identifier namespace_body ';'?
    ;

qualified_identifier
    : Identifier ('.' Identifier)*
    ;

namespace_body
    : '{' extern_alias_directive* using_directive* namespace_member_declaration* '}'
    ;

// Source: §14.4 Extern alias directives
extern_alias_directive
    : 'extern' 'alias' Identifier ';'
    ;

// Source: §14.5.1 General
using_directive
    : using_alias_directive
    | using_namespace_directive
    ;

// Source: §14.5.2 Using alias directives
using_alias_directive
    : 'using' Identifier '=' namespace_or_type_name ';'
    ;

// Source: §14.5.3 Using namespace directives
using_namespace_directive
    : 'using' namespace_name ';'
    ;

// Source: §14.6 Namespace member declarations
namespace_member_declaration
    : namespace_declaration
    | type_declaration
    ;

// Source: §14.7 Type declarations
type_declaration
    : class_declaration
    | struct_declaration
    | interface_declaration
    | enum_declaration
    | delegate_declaration
    ;

// Source: §14.8.1 General
qualified_alias_member
    : Identifier '::' Identifier type_argument_list?
    ;

// Source: §15.2.1 General
class_declaration
  : attributes? class_modifier* 'partial'? 'class' Identifier type_parameter_list?
  class_base? type_parameter_constraints_clause* class_body ';'?
  ;

// Source: §15.2.2.1 General
class_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    | 'abstract'
    | 'sealed'
    | 'static'
    ;

// Source: §15.2.3 Type parameters
type_parameter_list
  : '<' type_parameters '>'
  ;

type_parameters
  : attributes? type_parameter
  | type_parameters ',' attributes? type_parameter
  ;

// Source: §15.2.4.1 General
class_base
  : ':' class_type
  | ':' interface_type_list
  | ':' class_type ',' interface_type_list
  ;

interface_type_list
  : interface_type (',' interface_type)*
  ;

// Source: §15.2.5 Type parameter constraints
type_parameter_constraints_clauses
    : type_parameter_constraints_clause
    | type_parameter_constraints_clauses type_parameter_constraints_clause
    ;
    
type_parameter_constraints_clause
    : 'where' type_parameter ':' type_parameter_constraints
    ;

type_parameter_constraints
    : primary_constraint
    | secondary_constraints
    | constructor_constraint
    | primary_constraint ',' secondary_constraints
    | primary_constraint ',' constructor_constraint
    | secondary_constraints ',' constructor_constraint
    | primary_constraint ',' secondary_constraints ',' constructor_constraint
    ;

primary_constraint
    : class_type
    | 'class'
    | 'struct'
    ;

secondary_constraints
    : interface_type
    | type_parameter
    | secondary_constraints ',' interface_type
    | secondary_constraints ',' type_parameter
    ;

constructor_constraint
    : 'new' '(' ')'
    ;

// Source: §15.2.6 Class body
class_body
  : '{' class_member_declaration* '}'
  ;

// Source: §15.3.1 General
class_member_declaration
    : constant_declaration
    | field_declaration
    | method_declaration
    | property_declaration
    | event_declaration
    | indexer_declaration
    | operator_declaration
    | constructor_declaration
    | finalizer_declaration
    | static_constructor_declaration
    | type_declaration
    ;

// Source: §15.4 Constants
constant_declaration
    : attributes? constant_modifier* 'const' type constant_declarators ';'
    ;

constant_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    ;

// Source: §15.5.1 General
field_declaration
    : attributes? field_modifier* type variable_declarators ';'
    ;

field_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    | 'static'
    | 'readonly'
    | 'volatile'
    ;

variable_declarators
    : variable_declarator (',' variable_declarator)*
    ;

variable_declarator
    : Identifier ('=' variable_initializer)?
    ;

// Source: §15.6.1 General
method_declaration
    : method_header method_body
    ;

method_header
    : attributes? method_modifier* 'partial'? return_type member_name type_parameter_list? '(' formal_parameter_list? ')' type_parameter_constraints_clause*
    ;

method_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    | 'static'
    | 'virtual'
    | 'sealed'
    | 'override'
    | 'abstract'
    | 'extern'
    | 'async'
    ;

return_type
    : type
    | 'void'
    ;

member_name
    : Identifier
    | interface_type '.' Identifier
    ;

method_body
    : block
    ;

// Source: §15.6.2.1 General
formal_parameter_list
    : fixed_parameters
    | fixed_parameters ',' parameter_array
    | parameter_array
    ;

fixed_parameters
    : fixed_parameter (',' fixed_parameter)*
    ;

fixed_parameter
    : attributes? parameter_modifier? type Identifier default_argument?
    ;

default_argument
    : '=' expression
    ;

parameter_modifier
    : parameter_mode_modifier
    | 'this'
    ;

parameter_mode_modifier
    : 'ref'
    | 'out'
    ;

parameter_array
    : attributes? 'params' array_type Identifier
    ;

// Source: §15.7.1 General
property_declaration
    : attributes? property_modifiers? type member_name '{' accessor_declarations '}'
    ;

property_modifiers
    : property_modifier
    | property_modifiers property_modifier
    ;

property_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    | 'static'
    | 'virtual'
    | 'sealed'
    | 'override'
    | 'abstract'
    | 'extern'
    ;

// Source: §15.7.3 Accessors
accessor_declarations
   : get_accessor_declaration set_accessor_declaration?
   | set_accessor_declaration get_accessor_declaration?
   ;

get_accessor_declaration
    : attributes? accessor_modifier? 'get' accessor_body
    ;

set_accessor_declaration
    : attributes? accessor_modifier? 'set' accessor_body
    ;

accessor_modifier
    : 'protected'
    | 'internal'
    | 'private'
    | 'protected' 'internal'
    | 'internal' 'protected'
    ;

accessor_body
    : block
    | ';' 
    ;

// Source: §15.8.1 General
event_declaration
  : attributes? event_modifier* 'event' type variable_declarators ';'
  | attributes? event_modifier* 'event' type member_name '{' event_accessor_declarations '}'
  ;

event_modifier
  : 'new'
  | 'public'
  | 'protected'
  | 'internal'
  | 'private'
  | 'static'
  | 'virtual'
  | 'sealed'
  | 'override'
  | 'abstract'
  | 'extern'
  ;

event_accessor_declarations
  : add_accessor_declaration remove_accessor_declaration
  | remove_accessor_declaration add_accessor_declaration
  ;

add_accessor_declaration
  : attributes? 'add' block
  ;

remove_accessor_declaration
  : attributes? 'remove' block
  ;

// Source: §15.9 Indexers
indexer_declaration
  : attributes? indexer_modifiers? indexer_declarator '{' accessor_declarations '}'
  ;

indexer_modifiers
  : indexer_modifier
  | indexer_modifiers indexer_modifier
  ;

indexer_modifier
  : 'new'
  | 'public'
  | 'protected'
  | 'internal'
  | 'private'
  | 'virtual'
  | 'sealed'
  | 'override'
  | 'abstract'
  | 'extern'
  ;

indexer_declarator
  : type 'this' '[' formal_parameter_list ']'
  | type interface_type '.' 'this' '[' formal_parameter_list ']'
  ;

// Source: §15.10.1 General
operator_declaration
  : attributes? operator_modifier+ operator_declarator operator_body
  ;

operator_modifier
  : 'public'
  | 'static'
  | 'extern'
  ;

operator_declarator
  : unary_operator_declarator
  | binary_operator_declarator
  | conversion_operator_declarator
  ;

unary_operator_declarator
  : type 'operator' overloadable_unary_operator '(' fixed_parameter ')'
  ;

overloadable_unary_operator
  : '+' | '-' | '!' | '~' | '++' | '--' | 'true' | 'false'
  ;

binary_operator_declarator
  : type 'operator' overloadable_binary_operator '(' fixed_parameter ',' fixed_parameter ')'
  ;

overloadable_binary_operator
  : '+'  | '-'  | '*'  | '/'  | '%'  | '&' | '|' | '^'  | '<<' 
  | Right_shift | '==' | '!=' | '>' | '<' | '>=' | '<='
  ;

conversion_operator_declarator
  : 'implicit' 'operator' type '(' fixed_parameter ')'
  | 'explicit' 'operator' type '(' fixed_parameter ')'
  ;

operator_body
  : block
  | ';'
  ;

// Source: §15.11.1 General
constructor_declaration
  : attributes? constructor_modifier* constructor_declarator constructor_body
  ;

constructor_modifier
  : 'public'
  | 'protected'
  | 'internal'
  | 'private'
  | 'extern'
  ;

constructor_declarator
  : Identifier '(' formal_parameter_list? ')' constructor_initializer?
  ;

constructor_initializer
  : ':' 'base' '(' argument_list? ')'
  | ':' 'this' '(' argument_list? ')'
  ;

constructor_body
  : block
  | ';'
  ;

// Source: §15.12 Static constructors
static_constructor_declaration
  : attributes? static_constructor_modifiers Identifier '(' ')' static_constructor_body
  ;

static_constructor_modifiers
  : 'extern'? 'static'
  | 'static' 'extern'?
  ;

static_constructor_body
  : block
  | ';'
  ;

// Source: §15.13 Finalizers
finalizer_declaration
    : attributes? 'extern'? '~' Identifier '(' ')' finalizer_body
    ;

finalizer_body
    : block
    | ';'
    ;

// Source: §16.2.1 General
struct_declaration
    : attributes? struct_modifier* 'partial'? 'struct' Identifier type_parameter_list?
      struct_interfaces? type_parameter_constraints_clause* struct_body ';'?
    ;

// Source: §16.2.2 Struct modifiers
struct_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    ;

// Source: §16.2.4 Struct interfaces
struct_interfaces
    : ':' interface_type_list
    ;

// Source: §16.2.5 Struct body
struct_body
    : '{' struct_member_declaration* '}'
    ;

// Source: §16.3 Struct members
struct_member_declaration
    : constant_declaration
    | field_declaration
    | method_declaration
    | property_declaration
    | event_declaration
    | indexer_declaration
    | operator_declaration
    | constructor_declaration
    | static_constructor_declaration
    | type_declaration
    ;

// Source: §17.7 Array initializers
array_initializer
    : '{' variable_initializer_list? '}'
    | '{' variable_initializer_list ',' '}'
    ;

variable_initializer_list
    : variable_initializer (',' variable_initializer)*
    ;
    
variable_initializer
    : expression
    | array_initializer
    ;

// Source: §18.2.1 General
interface_declaration
    : attributes? interface_modifier* 'partial'? 'interface' Identifier variant_type_parameter_list? interface_base? type_parameter_constraints_clause* interface_body ';'?
    ;

// Source: §18.2.2 Interface modifiers
interface_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    ;

// Source: §18.2.3.1 General
variant_type_parameter_list
    : '<' variant_type_parameters '>'
    ;

// Source: §18.2.3.1 General
variant_type_parameters
    : attributes? variance_annotation? type_parameter
    | variant_type_parameters ',' attributes? variance_annotation? type_parameter
    ;

// Source: §18.2.3.1 General
variance_annotation
    : 'in'
    | 'out'
    ;

// Source: §18.2.4 Base interfaces
interface_base
    : ':' interface_type_list
    ;

// Source: §18.3 Interface body
interface_body
    : '{' interface_member_declaration* '}'
    ;

// Source: §18.4.1 General
interface_member_declaration
    : interface_method_declaration
    | interface_property_declaration
    | interface_event_declaration
    | interface_indexer_declaration
    ;

// Source: §18.4.2 Interface methods
interface_method_declaration
    : attributes? 'new'? return_type Identifier type_parameter_list? '(' formal_parameter_list? ')' type_parameter_constraints_clause* ';'
    ;

// Source: §18.4.3 Interface properties
interface_property_declaration
    : attributes? 'new'? type Identifier '{' interface_accessors '}'
    ;

// Source: §18.4.3 Interface properties
interface_accessors
    : attributes? 'get' ';'
    | attributes? 'set' ';'
    | attributes? 'get' ';' attributes? 'set' ';'
    | attributes? 'set' ';' attributes? 'get' ';'
    ;

// Source: §18.4.4 Interface events
interface_event_declaration
    : attributes? 'new'? 'event' type Identifier ';'
    ;

// Source: §18.4.5 Interface indexers
interface_indexer_declaration:
    attributes? 'new'? type 'this' '[' formal_parameter_list ']' '{' interface_accessors '}'
    ;

// Source: §19.2 Enum declarations
enum_declaration
    : attributes? enum_modifier* 'enum' Identifier enum_base? enum_body ';'?
    ;

enum_base
    : ':' struct_type
    ;

enum_body
    : '{' enum_member_declarations? '}'
    | '{' enum_member_declarations ',' '}'
    ;

// Source: §19.3 Enum modifiers
enum_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    ;

// Source: §19.4 Enum members
enum_member_declarations
    : enum_member_declaration (',' enum_member_declaration)*
    ;

// Source: §19.4 Enum members
enum_member_declaration
    : attributes? Identifier ('=' constant_expression)?
    ;

// Source: §20.2 Delegate declarations
delegate_declaration
    : attributes? delegate_modifier* 'delegate' return_type Identifier variant_type_parameter_list? '(' formal_parameter_list? ')' type_parameter_constraints_clause* ';'
    ;
    
delegate_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    ;

// Source: §22.3 Attribute specification
global_attributes
    : global_attribute_section+
    ;

global_attribute_section
    : '[' global_attribute_target_specifier attribute_list ']'
    | '[' global_attribute_target_specifier attribute_list ',' ']'
    ;

global_attribute_target_specifier
    : global_attribute_target ':'
    ;

global_attribute_target
    : Identifier
    ;

attributes
    : attribute_section+
    ;

attribute_section
    : '[' attribute_target_specifier? attribute_list ']'
    | '[' attribute_target_specifier? attribute_list ',' ']'
    ;

attribute_target_specifier
    : attribute_target ':'
    ;

attribute_target
    : Identifier
    | Keyword
    ;

attribute_list
    : attribute (',' attribute)*
    ;

attribute
    : attribute_name attribute_arguments?
    ;

attribute_name
    : type_name
    ;

attribute_arguments
    : '(' positional_argument_list? ')'
    | '(' positional_argument_list ',' named_argument_list ')'
    | '(' named_argument_list ')'
    ;

positional_argument_list
    : positional_argument (',' positional_argument)*
    ;

positional_argument
    : argument_name? attribute_argument_expression
    ;

named_argument_list
    : named_argument (','  named_argument)*
    ;

named_argument
    : Identifier '=' attribute_argument_expression
    ;

attribute_argument_expression
    : expression
    ;
```

## A.4 Grammar extensions for unsafe code

```ANTLR

// Source: §23.2 Unsafe contexts
class_modifier
    : ...
    | 'unsafe'
    ;

struct_modifier
    : ...
    | 'unsafe'
    ;

interface_modifier
    : ...
    | 'unsafe'
    ;

delegate_modifier
    : ...
    | 'unsafe'
    ;

field_modifier
    : ...
    | 'unsafe'
    ;

method_modifier
    : ...
    | 'unsafe'
    ;

property_modifier
    : '...'
    | 'unsafe'
    ;

event_modifier
    : ...
    | 'unsafe'

indexer_modifier
    : ...
    | 'unsafe'
    ;

operator_modifier
    : ...
    | 'unsafe'
    ;

constructor_modifier
    : ...
    | 'unsafe'
    ;

finalizer_declaration
    : attributes? 'extern'? 'unsafe'? '~' identifier '(' ')' finalizer_body
    | attributes? 'unsafe'? 'extern'? '~' identifier '(' ')' finalizer_body
    ;

static_constructor_modifiers
    : 'extern'? 'unsafe'? 'static'
    | 'unsafe'? 'extern'? 'static'
    | 'extern'? 'static' 'unsafe'?
    | 'unsafe'? 'static' 'extern'?
    | 'static' 'extern'? 'unsafe'?
    | 'static' 'unsafe'? 'extern'?
    ;

embedded_statement
    : ...
    | unsafe_statement
    | fixed_statement
    ;

unsafe_statement
    : 'unsafe' block
    ;

// Source: §23.3 Pointer types
type
    : ...
    | pointer_type
    ;

non_array_type
    : ...
    | pointer_type
    ;

// Source: §23.3 Pointer types
pointer_type
    : unmanaged_type '*'
    | 'void' '*'
    ;

unmanaged_type
    : type
    ;

// Source: §23.6.1 General
primary_no_array_creation_expression
    : ...
    | pointer_member_access
    | pointer_element_access
    ;

unary_expression
    : ...
    | pointer_indirection_expression
    | addressof_expression
    ;

// Source: §23.6.2 Pointer indirection
pointer_indirection_expression
    : '*' unary_expression
    ;

// Source: §23.6.3 Pointer member access
pointer_member_access
    : primary_expression '->' identifier type_argument_list?
    ;

// Source: §23.6.4 Pointer element access
pointer_element_access
    : primary_no_array_creation_expression '[' expression ']'
    ;

// Source: §23.6.5 The address-of operator
addressof_expression
    : '&' unary_expression
    ;

// Source: §23.7 The fixed statement
fixed_statement
    : 'fixed' '(' pointer_type fixed_pointer_declarators ')' embedded_statement
    ;

fixed_pointer_declarators
    : fixed_pointer_declarator (','  fixed_pointer_declarator)*
    ;

fixed_pointer_declarator
    : identifier '=' fixed_pointer_initializer
    ;

fixed_pointer_initializer
    : '&' variable_reference
    | expression
    ;

// Source: §23.8.2 Fixed-size buffer declarations
struct_member_declaration
    : ...
    | fixed_size_buffer_declaration
    ;

fixed_size_buffer_declaration
    : attributes? fixed_size_buffer_modifier* 'fixed' buffer_element_type fixed_size_buffer_declarator+ ';'
    ;

fixed_size_buffer_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    | 'unsafe'
    ;

buffer_element_type
    : type
    ;

fixed_size_buffer_declarator
    : identifier '[' constant_expression ']'
    ;

// Source: §23.9 Stack allocation
local_variable_initializer
    : ...
    | stackalloc_initializer
    ;

stackalloc_initializer
    : 'stackalloc' unmanaged_type '[' expression ']'
    ;
```

**End of informative text.**
