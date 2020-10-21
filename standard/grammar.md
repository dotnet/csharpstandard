# Annex A Grammar

**This clause is informative.**

## A.1 General

This annex contains the grammar productions found in the main document, including the optional ones for unsafe code. Productions appear here in the same order that they appear in the main document.

## A.2 Lexical and syntactic grammar

```ANTLR
input
    : input_section?
    ;

input_section
    : input_section_part+
    ;

input_section_part
    : input_element* new_line
    | pp_directive
    ;

input_element
    : whitespace
    | comment
    | token
    ;

new_line
    : '<Carriage return character (U+000D)>'
    | '<Line feed character (U+000A)>'
    | '<Carriage return character (U+000D) followed by line feed character (U+000A)>'
    | '<Next line character (U+0085)>'
    | '<Line separator character (U+2028)>'
    | '<Paragraph separator character (U+2029)>'
    ;

comment
    : single_line_comment
    | delimited_comment
    ;

single_line_comment
    : '//' input_characters?
    ;

input_characters
    : input_character
    | input_characters input_character
    ;

input_character
    : '<Any Unicode character except a new_line_character>'
    ;
    
new_line_character
    : '<Carriage return character (U+000D)>'
    | '<Line feed character (U+000A)>'
    | '<Next line character (U+0085)>'
    | '<Line separator character (U+2028)>'
    | '<Paragraph separator character (U+2029)>'
    ;
    
delimited_comment
    : '/*' delimited_comment_text? asterisks '/'
    ;

delimited_comment_text
    : delimited_comment_section
    | delimited_comment_text delimited_comment_section
    ;

delimited_comment_section
    : '/'
    | asterisks? not_slash_or_asterisk
    ;

asterisks
    : '*'
    | asterisks '*'
    ;

not_slash_or_asterisk
    : '<Any Unicode character except / or *>'
    ;

whitespace
    : whitespace_character
    | whitespace whitespace_character
    ;

whitespace_character
    : '<Any character with Unicode class Zs>'
    | '<Horizontal tab character (U+0009)>'
    | '<Vertical tab character (U+000B)>'
    | '<Form feed character (U+000C)>'
    ;

token
    : identifier
    | keyword
    | integer_literal
    | real_literal
    | character_literal
    | string_literal
    | operator_or_punctuator
    ;

unicode_escape_sequence
    : '\\u' hex_digit hex_digit hex_digit hex_digit
    | '\\U' hex_digit hex_digit hex_digit hex_digit hex_digit hex_digit hex_digit hex_digit
    ;

identifier
    : available_identifier
    | '@' identifier_or_keyword
    ;

available_identifier
    : '<An identifier_or_keyword that is not a keyword>'
    ;

identifier_or_keyword
    : identifier_start_character identifier_part_character?
    ;

identifier_start_character
    : letter_character
    | underscore_character
    ;

underscore_character
    : '<_ the underscore character (U+005F)>'
    | '<A unicode_escape_sequence representing the character U+005F>'
    ;

identifier_part_characters
    : identifier_part_character
    | identifier_part_characters identifier_part_character
    ;

identifier_part_character
    : letter_character
    | decimal_digit_character
    | connecting_character
    | combining_character
    | formatting_character
    ;

letter_character
    : '<A Unicode character of classes Lu, Ll, Lt, Lm, Lo, or Nl>'
    | '<A unicode-escape-sequence representing a character of classes Lu, Ll, Lt, Lm, Lo, or Nl>'
    ;

combining_character
    : '<A Unicode character of classes Mn or Mc>'
    | '<A unicode-escape-sequence representing a character of classes Mn or Mc>'
    ;

decimal_digit_character
    : '<A Unicode character of the class Nd>'
    | '<A unicode-escape-sequence representing a character of the class Nd>'
    ;

connecting_character
    : '<A Unicode character of the class Pc>'
    | '<A unicode-escape-sequence representing a character of the class Pc>'
    ;

formatting_character
    : '<A Unicode character of the class Cf>'
    | '<A unicode-escape-sequence representing a character of the class Cf>'
    ;

keyword
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

contextual_keyword
    : 'add'    'alias'         'ascending'   'async'      'await'
    | 'by'     'descending'    'dynamic'     'equals'     'from'
    | 'get'    'global'        'group'       'into'       'join'
    | 'let'    'orderby'       'partial'     'remove'     'select'
    | 'set'    'value'         'var'         'where'      'yield'
    ;

literal
    : boolean_literal
    | integer_literal
    | real_literal
    | character_literal
    | string_literal
    | null_literal
    ;

boolean_literal
    : 'true'
    | 'false'
    ;

integer_literal
    : decimal_integer_literal
    | hexadecimal_integer_literal
    ;

decimal_integer_literal
    : decimal_digits integer_type_suffix?
    ;
    
decimal_digits
    : decimal_digit
    | decimal_digits decimal_digit
    ;

decimal_digit
    : '0' | '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9'
    ;
    
integer_type_suffix
    : 'U' | 'u' | 'L' | 'l' | 'UL' | 'Ul' | 'uL' | 'ul' | 'LU' | 'Lu' | 'lU' | 'lu'
    ;
    
hexadecimal_integer_literal
    : '0x' hex_digits integer_type_suffix?
    | '0X' hex_digits integer_type_suffix?
    ;

hex_digits
    : hex_digit
    | hex_digits hex_digit
    ;

hex_digit
    : '0' | '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9'
    | 'A' | 'B' | 'C' | 'D' | 'E' | 'F' | 'a' | 'b' | 'c' | 'd' | 'e' | 'f';

real_literal
    : decimal_digits '.' decimal_digits exponent_part? real_type_suffix?
    | '.' decimal_digits exponent_part? real_type_suffix?
    | decimal_digits exponent_part real_type_suffix?
    | decimal_digits real_type_suffix
    ;

exponent_part
    : 'e' sign? decimal_digits
    | 'E' sign? decimal_digits
    ;

sign
    : '+' | '-'
    ;

real_type_suffix
    : 'F' | 'f' | 'D' | 'd' | 'M' | 'm'
    ;

character_literal
    : '\'' character '\''
    ;
    
character
    : single_character
    | simple_escape_sequence
    | hexadecimal_escape_sequence
    | unicode_escape_sequence
    ;
    
single_character
    : '<Any character except \' (U+0027), \\ (U+005C), and new_line_character>'
    ;
    
simple_escape_sequence
    : '\\\'' | '\\"' | '\\\\' | '\\0' | '\\a' | '\\b' | '\\f' | '\\n' | '\\r' | '\\t' | '\\v'
    ;
    
hexadecimal_escape_sequence
    : '\\x' hex_digit hex_digit? hex_digit? hex_digit?;

string_literal
    : regular_string_literal
    | verbatim_string_literal
    ;
    
regular_string_literal
    : '"' regular_string_literal_characters? '"'
    ;
    
regular_string_literal_characters
    : regular_string_literal_character
    | regular_string_literal_characters regular_string_literal_character
    ;

regular_string_literal_character
    : single_regular_string_literal_character
    | simple_escape_sequence
    | hexadecimal_escape_sequence
    | unicode_escape_sequence
    ;

single_regular_string_literal_character
    : '<Any character except \" (U+0022), \\ (U+005C), and new_line_character>'
    ;

verbatim_string_literal
    : '@"' verbatim_string_literal_characters? '"'
    ;
    
verbatim_string_literal_characters
    : verbatim_string_literal_character
    | verbatim_string_literal_characters verbatim_string_literal_character
    ;

verbatim_string_literal_character
    : single_verbatim_string_literal_character
    | quote_escape_sequence
    ;
    
single_verbatim_string_literal_character
    : '<any character except ">'
    ;
    
quote_escape_sequence
    : '""'
    ;

null_literal
    : 'null'
    ;

operator_or_punctuator
    : '{'  | '}'  | '['  | ']'  | '('   | ')'  | '.'  | ','  | ':'  | ';'
    | '+'  | '-'  | '*'  | '/'  | '%'   | '&'  | '|'  | '^'  | '!'  | '~'
    | '='  | '<'  | '>'  | '?'  | '??'  | '::' | '++' | '--' | '&&' | '||'
    | '->' | '==' | '!=' | '<=' | '>='  | '+=' | '-=' | '*=' | '/=' | '%='
    | '&=' | '|=' | '^=' | '<<' | '<<=' | '=>'
    ;
    
right_shift
    : '>'  '>'
    ;

right_shift_assignment
    : '>' '>='
    ;

pp_directive
    : pp_declaration
    | pp_conditional
    | pp_line
    | pp_diagnostic
    | pp_region
    | pp_pragma
    ;

conditional_symbol
    : '<Any identifier_or_keyword except true or false>'
    ;

pp_expression
    : whitespace? pp_or_expression whitespace?
    ;
    
pp_or_expression
    : pp_and_expression
    | pp_or_expression whitespace? '||' whitespace? pp_and_expression
    ;
    
pp_and_expression
    : pp_equality_expression
    | pp_and_expression whitespace? '&&' whitespace? pp_equality_expression
    ;

pp_equality_expression
    : pp_unary_expression
    | pp_equality_expression whitespace? '==' whitespace? pp_unary_expression
    | pp_equality_expression whitespace? '!=' whitespace? pp_unary_expression
    ;
    
pp_unary_expression
    : pp_primary_expression
    | '!' whitespace? pp_unary_expression
    ;
    
pp_primary_expression
    : 'true'
    | 'false'
    | conditional_symbol
    | '(' whitespace? pp_expression whitespace? ')'
    ;

pp_declaration
    : whitespace? '#' whitespace? 'define' whitespace conditional_symbol pp_new_line
    | whitespace? '#' whitespace? 'undef' whitespace conditional_symbol pp_new_line
    ;

pp_new_line
    : whitespace? single_line_comment? new_line
    ;

pp_conditional
    : pp_if_section pp_elif_sections? pp_else_section? pp_endif
    ;

pp_if_section
    : whitespace? '#' whitespace? 'if' whitespace pp_expression pp_new_line conditional_section?
    ;
    
pp_elif_sections
    : pp_elif_section
    | pp_elif_sections pp_elif_section
    ;

pp_elif_section
    : whitespace? '#' whitespace? 'elif' whitespace pp_expression pp_new_line conditional_section?
    ;
    
pp_else_section:
    | whitespace? '#' whitespace? 'else' pp_new_line conditional_section?
    ;
    
pp_endif
    : whitespace? '#' whitespace? 'endif' pp_new_line
    ;
    
conditional_section
    : input_section
    | skipped_section
    ;
    
skipped_section
    : skipped_section_part
    | skipped_section skipped_section-part
    ;

skipped_section_part
    : skipped_characters? new_line
    | pp_directive
    ;
    
skipped_characters
    : whitespace? not_number_sign input_characters?
    ;

not_number_sign
    : '<Any input_character except #>'
    ;

pp_diagnostic
    : whitespace? '#' whitespace? 'error' pp_message
    | whitespace? '#' whitespace? 'warning' pp_message
    ;

pp_message
    : new_line
    | whitespace input_characters? new_line
    ;

pp_region
    : pp_start_region conditional_section? pp_end_region
    ;

pp_start_region
    : whitespace? '#' whitespace? 'region' pp_message
    ;

pp_end_region
    : whitespace? '#' whitespace? 'endregion' pp_message
    ;

pp_line
    : whitespace? '#' whitespace? 'line' whitespace line_indicator pp_new_line
    ;

line_indicator
    : decimal_digits whitespace file_name
    | decimal_digits
    | 'default'
    | 'hidden'
    ;
    
file_name
    : '"' file_name_characters '"'
    ;
    
file_name_characters
    : file_name_characters file_name_character
    ;

file_name_character
    : <Any input_character except \" (U+0022), and new-line-character>
    ;

pp-pragma
    : whitespace? '#' whitespace? 'pragma' pp_pragma_text
    ;

pp_pragma_text
    : new_line
    | whitespace input_characters? new_line
    ;

type
    : reference_type
    | value_type
    | type_parameter
    ;

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
    : non_array_type rank_specifiers
    ;

non_array_type
    : value_type
    | class_type
    | interface_type
    | delegate_type
    | 'dynamic'
    | type_parameter
    ;

rank_specifiers
    : rank_specifier
    | rank_specifiers rank_specifier
    ;

rank_specifier
    : '[' dim_separators? ']'
    ;

dim_separators
    : ','
    | dim_separators ','
    ;

delegate_type
    : type_name
    ;

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

nullable_type
    : non_nullable_value_type '?'
    ;

non_nullable_value_type
    : type
    ;

enum_type
    : type_name
    ;

type_argument_list
    : '<' type_arguments '>'
    ;

type_arguments
    : type_argument
    | type_arguments ',' type_argument
    ;   

type_argument
    : type
    ;

type_parameter
    : identifier
    ;

variable_reference
    : expression
    ;

argument_list
    : argument
    | argument_list ',' argument
    ;

argument
    : argument_name? argument_value
    ;

argument_name
    : identifier ':'
    ;

argument_value
    : expression
    | 'ref' variable_reference
    | 'out' variable_reference
    ;

primary_expression
    : primary_no_array_creation_expression
    | array_creation_expression
    ;

primary_no_array_creation_expression
    : literal
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

simple_name
    : identifier type_argument_list?
    ;

parenthesized_expression
    : '(' expression ')'
    ;

member_access
    : primary_expression '.' identifier type_argument_list?
    | predefined_type '.' identifier type_argument_list?
    | qualified_alias_member '.' identifier type_argument_list?
    ;

predefined_type
    : 'bool'   | 'byte'  | 'char'  | 'decimal' | 'double' | 'float' | 'int' | 'long'
    | 'object' | 'sbyte' | 'short' | 'string'  | 'uint'   | 'ulong' | 'ushort'
    ;

invocation_expression
    : primary-expression '(' argument_list? ')'
    ;

element_access
    : primary_no_array_creation_expression '[' argument_list ']'
    ;

this_access
    : 'this'
    ;

base_access
    : 'base' '.' identifier type_argument_list?
    : 'base' '[' argument_list ']'
    ;

post_increment_expression
    : primary_expression '++'
    ;

post_decrement_expression
    : primary_expression '--'
    ;

object_creation_expression
    : 'new' type '(' argument_list? ')' object_or_collection_initializer?
    | 'new' type object_or_collection_initializer
    ;

object_or_collection_initializer
    : object_initializer
    | collection_initializer
    ;

object_initializer
    : '{' member_initializer_list? '}'
    | '{' member_initializer_list ',' '}'
    ;

member_initializer_list
    : member_initializer
    | member_initializer_list ',' member_initializer
    ;

member_initializer
    : identifier '=' initializer_value
    ;

initializer_value
    : expression
    | object_or_collection_initializer
    ;

collection_initializer
    : '{' element_initializer_list '}'
    | '{' element_initializer_list ',' '}'
    ;

element_initializer_list
    : element_initializer
    | element_initializer_list ',' element_initializer
    ;

element_initializer
    : non_assignment_expression
    | '{' expression_list '}'
    ;

expression_list
    : expression
    | expression_list ',' expression
    ;

array_creation_expression
    : 'new' non_array_type '[' expression_list ']' rank_specifiers? array_initializer?
    | 'new' array_type array_initializer
    | 'new' rank_specifier array_initializer
    ;

delegate_creation_expression
    : 'new' delegate_type '(' expression ')'
    ;

anonymous_object_creation_expression
    : 'new' anonymous_object_initializer
    ;

anonymous_object_initializer
    : '{' member_declarator_list? '}'
    | '{' member_declarator_list ',' '}'
    ;

member_declarator_list
    : member_declarator
    | member_declarator_list ',' member_declarator
    ;

member_declarator
    : simple_name
    | member_access
    | base_access
    | identifier '=' expression
    ;

typeof_expression
    : 'typeof' '(' type ')'
    | 'typeof' '(' unbound_type_name ')'
    | 'typeof' '(' 'void' ')'
    ;

unbound_type_name
    : identifier generic_dimension_specifier?
    | identifier '::' identifier generic_dimension_specifier?
    | unbound_type_name '.' identifier generic_dimension_specifier?
    ;

generic_dimension_specifier
    : '<' commas? '>'
    ;

commas
    : ','
    | commas ','
    ;

checked_expression
    : 'checked' '(' expression ')'
    ;

unchecked_expression
    : 'unchecked' '(' expression ')'
    ;

default_value_expression
    : 'default' '(' type ')'
    ;

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

pre_increment_expression
    : '++' unary_expression
    ;

pre_decrement_expression
    : '--' unary_expression
    ;

cast_expression
    : '(' type ')' unary_expression
    ;

await_expression
    : 'await' unary_expression
    ;

multiplicative_expression
    : unary_expression
    | multiplicative_expression '*' unary_expression
    | multiplicative_expression '/' unary_expression
    | multiplicative_expression '%' unary_expression
    ;

additive_expression
    : multiplicative_expression
    | additive_expression '+' multiplicative_expression
    | additive_expression '–' multiplicative_expression
    ;

shift_expression
    : additive_expression
    | shift_expression '<<' additive_expression
    | shift_expression right_shift additive_expression
    ;

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

conditional_and_expression
    : inclusive_or_expression
    | conditional_and_expression '&&' inclusive_or_expression
    ;

conditional_or_expression
    : conditional_and_expression
    | conditional_or_expression '||' conditional_and_expression
    ;

null_coalescing_expression
    : conditional_or_expression
    | conditional_or_expression '??' null_coalescing_expression
    ;

conditional_expression
    : null_coalescing_expression
    | null_coalescing_expression '?' expression ':' expression
    ;

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
    : explicit_anonymous_function_parameter
    | explicit_anonymous_function_parameter_list ',' explicit_anonymous_function_parameter
    ;

explicit_anonymous_function_parameter
    : anonymous_function_parameter_modifier? type identifier
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
    : implicit_anonymous_function_parameter
    | implicit_anonymous_function_parameter_list ',' implicit_anonymous_function_parameter
    ;

implicit_anonymous_function_parameter
    : identifier
    ;

anonymous_function_body
    : expression
    | block
    ;

query_expression
    : from_clause query_body
    ;

from_clause
    : 'from' type? identifier 'in' expression
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
    : 'let' identifier '=' expression
    ;

where_clause
    : 'where' boolean_expression
    ;

join_clause
    : 'join' type? identifier 'in' expression 'on' expression 'equals' expression
    ;

join_into_clause
    : 'join' type? identifier 'in' expression 'on' expression 'equals' expression 'into' identifier
    ;

orderby_clause
    : 'orderby' orderings
    ;

orderings
    : ordering
    | orderings ',' ordering
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
    : 'into' identifier query_body
    ;

assignment
    : unary_expression assignment_operator expression
    ;

assignment_operator
    : '=' | '+=' | '-=' | '*=' | '/=' | '%=' | '&=' | '|=' | '^=' | '<<='
    | right_shift_assignment

expression
    : non_assignment_expression
    | assignment
    ;

non_assignment_expression
    : conditional_expression
    | lambda_expression
    | query_expression
    ;

constant_expression
    : expression
    ;

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

block
    : '{' statement_list? '}'
    ;

statement_list
    : statement
    | statement_list statement
    ;

empty_statement
    : ';'
    ;

labeled_statement
    : identifier ':' statement
    ;

declaration_statement
    : local_variable_declaration ';'
    | local_constant_declaration ';'
    ;

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
    : identifier
    | identifier '=' local_variable_initializer
    ;

local_variable_initializer
    : expression
    | array_initializer
    ;

local_constant_declaration
    : 'const' type constant_declarators
    ;

constant_declarators
    : constant_declarator
    | constant_declarators ',' constant_declarator
    ;

constant_declarator
    : identifier '=' constant_expression
    ;

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

selection_statement
    : if_statement
    | switch_statement
    ;

if_statement
    : 'if' '(' boolean_expression ')' embedded_statement
    | 'if' '(' boolean_expression ')' embedded_statement 'else' embedded_statement
    ;

switch_statement
    : 'switch' '(' expression ')' switch_block
    ;

switch_block
    : '{' switch_sections? '}'
    ;

switch_sections
    : switch_section
    | switch_sections switch_section
    ;

switch_section
    : switch_labels statement_list
    ;

switch_labels
    : switch_label
    | switch_labels switch_label
    ;

switch_label
    : 'case' constant_expression ':'
    | 'default' ':'
    ;

iteration_statement
    : while_statement
    | do_statement
    | for_statement
    | foreach_statement
    ;

while_statement
    : 'while' '(' boolean_expression ')' embedded_statement
    ;

do_statement
    : 'do' embedded_statement 'while' '(' boolean_expression ')' ';'

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
    : statement_expression
    | statement_expression_list ',' statement_expression
    ;

foreach_statement
    : 'foreach' '(' local_variable_type identifier 'in' expression ')' embedded_statement
    ;

jump_statement
    : break_statement
    | continue_statement
    | goto_statement
    | return_statement
    | throw_statement
    ;

break_statement
    : 'break' ';'
    ;

continue_statement
    : 'continue' ';'
    ;

goto_statement
    : 'goto' identifier ';'
    | 'goto' 'case' constant_expression ';'
    | 'goto' 'default' ';'
    ;

return_statement
    : 'return' expression? ';'
    ;

throw_statement
    : 'throw' expression? ';'
    ;

try_statement
    : 'try' block catch_clauses
    | 'try' block catch_clauses? finally_clause
    ;

catch_clauses
    : specific_catch_clauses
    | specific_catch_clauses? general_catch_clause
    ;

specific_catch_clauses
    : specific_catch_clause
    | specific_catch_clauses specific_catch_clause
    ;

specific_catch_clause
    : 'catch' '(' type identifier? ')' block
    ;

general_catch_clause
    : 'catch' block
    ;

finally_clause
    : 'finally' block
    ;

checked_statement
    : 'checked' block
    ;

unchecked_statement
    : 'unchecked' block
    ;

lock_statement
    : 'lock' '(' expression ')' embedded_statement
    ;

using_statement
    : 'using' '(' resource_acquisition ')' embedded_statement
    ;

resource_acquisition
    : local_variable_declaration
    | expression
    ;

yield_statement
    : 'yield' 'return' expression ';'
    | 'yield' 'break' ';'
    ;

compilation_unit
    : extern_alias_directives? using_directives? global_attributes? namespace_member_declarations?
    ;

namespace_declaration
    : 'namespace' qualified_identifier namespace_body ';'?
    ;

qualified_identifier
    : identifier
    | qualified_identifier '.' identifier
    ;

namespace_body
    : '{' extern_alias_directives? using_directives? namespace_member_declarations? '}'
    ;

extern_alias_directives
    : extern_alias_directive
    | extern_alias_directives extern_alias_directive
    ;

extern_alias_directive
    : 'extern' 'alias' identifier ';'
    ;

using_directives
    : using_directive
    | using_directives using_directive
    ;

using_directive
    : using_alias-directive
    | using_namespace-directive
    ;

using_alias_directive
    : 'using' identifier '=' namespace_or_type_name ';'
    ;

using_namespace_directive
    : 'using' namespace_name ';'
    ;

namespace_member_declarations
    : namespace_member_declaration
    | namespace_member_declarations namespace_member_declaration
    ;

namespace_member_declaration
    : namespace_declaration
    | type_declaration
    ;

type_declaration
    : class_declaration
    | struct_declaration
    | interface_declaration
    | enum_declaration
    | delegate_declaration
    ;

qualified_alias_member
    : identifier '::' identifier type_argument_list?
    ;

class_declaration
  : attributes? class_modifiers? 'partial'? 'class' identifier type_parameter_list?
  class_base? type_parameter_constraints_clauses? class_body ';'?
  ;

class_modifiers
    : class_modifier
    | class_modifiers class_modifier
    ;

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

type_parameter_list
  : '<' type_parameters '>'
  ;

type_parameters
  : attributes? type_parameter
  | type_parameters ',' attributes? type_parameter
  ;

class_base
  : ':' class_type
  | ':' interface_type_list
  | ':' class_type ',' interface_type_list
  ;

interface_type_list
  : interface_type
  | interface_type_list ',' interface_type
  ;

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

class_body
  : '{' class_member_declarations? '}'
  ;

class_member_declarations
    : class_member_declaration
    | class_member_declarations class_member_declaration
    ;

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

constant_declaration
    : attributes? constant_modifiers? 'const' type constant_declarators ';'
    ;

constant_modifiers
    : constant_modifier
    | constant_modifiers constant_modifier
    ;

constant_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    ;

constant_declarators
    : constant_declarator
    | constant_declarators ',' constant_declarator
    ;

constant_declarator
    : identifier '=' constant_expression
    ;

field_declaration
    : attributes? field_modifiers? type variable_declarators ';'
    ;

field_modifiers
    : field_modifier
    | field_modifiers field_modifier
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
    : variable_declarator
    : variable_declarators ',' variable_declarator
    ;

variable_declarator
    : identifier
    | identifier '=' variable_initializer
    ;

variable_initializer
    : expression
    | array_initializer
    ;

method_declaration
    : method_header method_body
    ;

method_header
    : attributes? method-modifiers? 'partial'? return-type member_name type_parameter_list? '(' formal_parameter_list? ')' type_parameter_constraints_clauses?
    ;

method_modifiers
    : method_modifier
    | method_modifiers method_modifier
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
    : identifier
    | interface_type '.' identifier
    ;

method_body
    : block
    ;

formal_parameter_list
    : fixed_parameters
    | fixed_parameters ',' parameter_array
    | parameter_array
    ;

fixed_parameters
    : fixed_parameter
    | fixed_parameters ',' fixed_parameter
    ;

fixed_parameter
    : attributes? parameter_modifier? type identifier default_argument?
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
    : attributes? 'params' array_type identifier
    ;

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

event_declaration
  : attributes? event_modifiers? 'event' type variable_declarators ';'
  | attributes? event_modifiers? 'event' type member_name '{' event_accessor_declarations '}'
  ;

event_modifiers
  : event_modifier
  | event_modifiers event_modifier
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

operator_declaration
  : attributes? operator_modifiers operator_declarator operator_body
  ;

operator_modifiers
  : operator_modifier
  | operator_modifiers operator_modifier
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
  | right_shift | '==' | '!=' | '>' | '<' | '>=' | '<='
  ;

conversion_operator_declarator
  : 'implicit' 'operator' type '(' fixed_parameter ')'
  | 'explicit' 'operator' type '(' fixed-parameter ')'
  ;

operator_body
  : block
  | ';'
  ;

constructor_declaration
  : attributes? constructor_modifiers? constructor_declarator constructor_body
  ;

constructor_modifiers
  : constructor_modifier
  | constructor_modifiers constructor_modifier
  ;

constructor_modifier
  : 'public'
  | 'protected'
  | 'internal'
  | 'private'
  | 'extern'
  ;

constructor_declarator
  : identifier '(' formal_parameter_list? ')' constructor_initializer?
  ;

constructor_initializer
  : ':' 'base' '(' argument_list? ')'
  | ':' 'this' '(' argument_list? ')'
  ;

constructor_body
  : block
  | ';'

static_constructor_declaration
  : attributes? static_constructor_modifiers identifier '(' ')' static_constructor_body
  ;

static_constructor_modifiers
  : 'extern'? 'static'
  | 'static' 'extern'?
  ;

static_constructor_body
  : block
  | ';'

finalizer_declaration
    : attributes? 'extern'? '~' identifier '(' ')' finalizer_body
    ;

finalizer_body
    : block
    | ';'

struct_declaration
    : attributes? struct_modifier* 'partial'? 'struct' identifier type_parameter_list?
      struct_interfaces? type_parameter_constraints_clause* struct_body ';'?
    ;

struct_modifiers
    : struct_modifier
    | struct_modifiers struct_modifier
    ;

struct_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    ;

struct_interfaces
    : ':' interface_type_list
    ;

struct_body
    : '{' struct_member_declarations? '}'
    ;

struct_member_declarations
    : struct_member_declaration
    | struct_member_declarations struct_member_declaration
    ;

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

array_initializer
    : '{' variable_initializer_list? '}'
    | '{' variable_initializer_list ',' '}'
    ;

variable_initializer_list
    : variable_initializer
    | variable_initializer_list ',' variable_initializer
    ;
    
variable_initializer
    : expression
    | array_initializer
    ;

interface_declaration
    : attributes? interface_modifiers? 'partial'? 'interface' identifier variant_type_parameter_list? interface_base? type_parameter_constraints_clauses? interface_body ';'?
    ;

interface_modifiers
    : interface_modifier
    | interface_modifiers interface_modifier
    ;

interface_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    ;

variant_type_parameter_list
    : '<' variant_type_parameters '>'
    ;

variant_type_parameters
    : attributes? variance_annotation? type_parameter
    | variant_type_parameters ',' attributes? variance_annotation? type_parameter
    ;

variance_annotation
    : 'in'
    | 'out'
    ;

interface_base
    : ':' interface_type_list
    ;

interface_body
    : '{' interface_member_declarations? '}'
    ;

interface_member_declarations
    : interface_member_declaration
    | interface_member_declarations interface_member_declaration
    ;

interface_member_declaration
    : interface_method_declaration
    | interface_property_declaration
    | interface_event_declaration
    | interface_indexer_declaration
    ;

interface_method_declaration
    : attributes? 'new'? return_type identifier type_parameter_list? '(' formal_parameter_list? ')' type_parameter_constraints_clauses? ';'
    ;

interface_property_declaration
    : attributes? 'new'? type identifier '{' interface_accessors '}'
    ;

interface_accessors
    : attributes? 'get' ';'
    | attributes? 'set' ';'
    | attributes? 'get' ';' attributes? 'set' ';'
    | attributes? 'set' ';' attributes? 'get' ';'
    ;

interface_event_declaration
    : attributes? 'new'? 'event' type identifier ';'
    ;

interface_indexer_declaration:
    attributes? 'new'? type 'this' '[' formal_parameter_list ']' '{' interface_accessors '}'
    ;

enum_declaration
    : attributes? enum_modifiers? 'enum' identifier enum_base? enum_body ';'?
    ;

enum_base
    : ':' integral_type
    ;

enum_body
    : '{' enum_member_declarations? '}'
    | '{' enum_member_declarations ',' '}'

enum_modifiers
    : enum_modifier
    | enum_modifiers enum_modifier
    ;

enum-modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    ;

enum_member_declarations
    : enum_member_declaration
    | enum_member_declarations ',' enum_member_declaration
    ;

enum_member_declaration
    : attributes? identifier
    | attributes? identifier '=' constant_expression
    ;

delegate_declaration
    : attributes? delegate_modifiers? 'delegate' return_type identifier variant_type_parameter_list? '(' formal_parameter_list? ')' type_parameter_constraints_clauses? ';'
    ;
    
delegate_modifiers
    : delegate_modifier
    | delegate_modifiers delegate_modifier
    ;
    
delegate_modifier
    : 'new'
    | 'public'
    | 'protected'
    | 'internal'
    | 'private'
    ;

global_attributes
    : global_attribute_sections
    ;

global_attribute_sections
    : global_attribute_section
    | global_attribute_sections global_attribute_section
    ;

global_attribute_section
    : '[' global_attribute_target_specifier attribute_list ']'
    | '[' global_attribute_target_specifier attribute_list ',' ']'
    ;

global_attribute_target_specifier
    : global_attribute_target ':'
    ;

global_attribute_target
    : identifier
    ;

attributes
    : attribute_sections
    ;

attribute_sections
    : attribute_section
    | attribute_sections attribute_section
    ;

attribute_section
    : '[' attribute_target_specifier? attribute_list ']'
    | '[' attribute_target_specifier? attribute_list ',' ']'
    ;

attribute_target_specifier
    : attribute_target ':'
    ;

attribute_target
    : identifier
    | keyword
    ;

attribute_list
    : attribute
    | attribute_list ',' attribute
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
    : positional_argument
    | positional_argument_list ',' positional_argument
    ;

positional_argument
    : argument_name? attribute_argument_expression
    ;

named_argument_list
    : named_argument
    | named_argument_list ',' named_argument
    ;

named_argument
    : identifier '=' attribute_argument_expression
    ;

attribute_argument_expression
    : expression
    ;

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

type
    : ...
    | pointer_type
    ;

non_array_type
    : ...
    | pointer_type
    ;

pointer_type
    : unmanaged_type '*'
    | 'void' '*'
    ;

unmanaged_type
    : type
    ;

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

pointer_indirection_expression
    : '*' unary_expression
    ;

pointer_member_access
    : primary_expression '->' identifier type_argument_list?
    ;

pointer_element_access
    : primary_no_array_creation_expression '[' expression ']'
    ;

addressof_expression
    : '&' unary_expression
    ;

fixed_statement
    : 'fixed' '(' pointer_type fixed_pointer_declarators ')' embedded_statement
    ;

fixed_pointer_declarators
    : fixed_pointer_declarator
    | fixed_pointer_declarators ',' fixed_pointer_declarator
    ;

fixed_pointer_declarator
    : identifier '=' fixed_pointer_initializer
    ;

fixed_pointer_initializer
    : '&' variable_reference
    | expression
    ;

struct_member_declaration
    : ...
    | fixed_size_buffer_declaration
    ;

fixed_size_buffer_declaration
    : attributes? fixed_size_buffer_modifiers? 'fixed' buffer_element_type fixed_size_buffer_declarators ';'
    ;

fixed_size_buffer_modifiers
    : fixed_size_buffer_modifier
    | fixed_size_buffer_modifier fixed_size_buffer_modifiers
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

fixed_size_buffer_declarators
    : fixed_size_buffer_declarator
    | fixed_size_buffer_declarator ',' fixed_size_buffer_declarators
    ;

fixed_size_buffer_declarator
    : identifier '[' constant_expression ']'
    ;

local_variable_initializer
    : ...
    | stackalloc_initializer
    ;

stackalloc_initializer
    : 'stackalloc' unmanaged_type '[' expression ']'
    ;
```

**End of informative text.**
