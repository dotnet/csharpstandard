extern alias Foo;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using M = System.Math;

using ConsoleApplication2.Test;

/**/
/* the previous comment is an empty delimited comment and not a document comment */
/** this is a document comment */
// this one is a single line comment

using X = int1;
using Y = ABC.X<int>;

using static System.Math;
using static System.DayOfWeek;
using static System.Linq.Enumerable;

[assembly: System.Copyright(@"(C)"" 

2009")]
[module: System.Copyright("\n\t\u0123(C) \"2009" + "\u0123")]

class TopLevelType : IDisposable
{
    void IDisposable.Dispose() { }
}