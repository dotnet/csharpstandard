// grammar understood by the ExampleExtractor tool when processing HTML comments containing example annotations.

grammar ExampleAnnotationGrammar;

example_annotation
    : '<!-- Example:' '{' annotation_directive (',' annotation_directive)* '}' '-->'
    ;

annotation_directive
    : template
    | name
    | replace_ellipsis
    | custom_ellipsis_replacements
    | expected_errors
    | expected_warnings
    | ignored_warnings
    | expected_output
    | infer_output
    | expected_exception
    | additional_files
    | extern_alias_support
    | execution_arguments
    ;

JSON_string_value
    : '"' ~["\u000D\u000A]+ '"'
    ;

WS : [ \t\r\n]+ -> skip ; // skip spaces, tabs, newlines, \r (Windows)

template
    : 'template' ':' template_name
    ;

template_name
    : '"code-in-class-lib"'                 // actually, a JSON_string_value with this content
    | '"code-in-class-lib-without-using"'   // actually, a JSON_string_value with this content
    | '"code-in-main"'                      // actually, a JSON_string_value with this content
    | '"code-in-main-without-using"'        // actually, a JSON_string_value with this content
    | '"code-in-partial-class"'             // actually, a JSON_string_value with this content
    | '"extern-lib"'                        // actually, a JSON_string_value with this content
    | '"standalone-console"'                // actually, a JSON_string_value with this content
    | '"standalone-lib"'                    // actually, a JSON_string_value with this content
    | '"standalone-lib-without-using"'      // actually, a JSON_string_value with this content
    ;

name
    : 'name' ':' test_filename
    ;

test_filename
    : JSON_string_value
    ;

replace_ellipsis
    : 'replaceEllipsis' ':' ('true' | 'false')
    ;

custom_ellipsis_replacements
    : 'customEllipsisReplacements' ':' '[' replacement (',' replacement)* ']'
    ;

replacement
    : 'null'
    | output_string
    ;

expected_errors
    : 'expectedErrors' ':' '[' cs_number (',' cs_number)* ']'
    ;

cs_number
    : JSON_string_value      // of the form "CSnnnn"
    ;

expected_warnings
    : 'expectedWarnings' ':' '[' cs_number (',' cs_number)* ']'
    ;

ignored_warnings
    : 'ignoredWarnings' ':' '[' cs_number (',' cs_number)* ']'
    ;

expected_output
    : 'expectedOutput' ':' '[' output_string (',' output_string)* ']'
    ;

infer_output
    : 'inferOutput' ':' ('true' | 'false')
    ;

expected_exception
    : 'expectedException' ':' exception_name
    ;

exception_name
    : JSON_string_value    // an unqualified type name
    ;

output_string
    : JSON_string_value
    ;
    
additional_files
    : 'additionalFiles' ':' '[' filename (',' filename)* ']'
    ;

extern_alias_support
    : 'project' ':' filename
    ;
    
execution_arguments
    : 'executionArgs' ':' '[' cmdlinearg (',' cmdlinearg)* ']'
    ;

cmdlinearg
    : JSON_string_value
    ;

filename
    : JSON_string_value
    ;
