class IndexAndRangeExpressions
{
   void IndexAndRangeExpressions_1()
   {
      var a = ^1;
      var b = ..^1;
      var c = 0..;
      var d = ..;
      var e = new Struct();      // type with ops * and / taking Range
      var f = e * 1 .. 2;
      var g = e * ..;
      var h = e * 1..;
      var i = e * ..2;
      var j = .. / e;
      var k = ^2.. / e;
      var l = .. ^1/e;
      var m = ^2 ..^1 / e;
      var n = 1..(.. 6).End;
      var o = b.Start..5;
      var p = (1..3).Start..5;

      var oops = (1..3)..5;      // compile-time type error
      var oops = 1..(3..5);      // compile-time type error
      var oops = 1..3..5;        // compile-time syntax error
   }
}