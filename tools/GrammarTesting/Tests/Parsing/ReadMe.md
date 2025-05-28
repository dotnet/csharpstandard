# Parsing Tests

There are two sets of tests. The standard set, in `Samples`, test whether the Standard’s
grammar is producing the expected parse.

The second set, in `Demo`, exist to demonstrate the system and are run when `SetupAndTest`
or `DoParsingTests` are passed the `-d` option.

If `Demo` is not here it will be in the environment parallel which can be located using the
environment variable `BG_LIB_PARSINGTESTS`.
