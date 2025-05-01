namespace Comments.XmlComments.UndocumentedKeywords
{
    // From here:https://github.com/dotnet/roslyn/wiki/New-Language-Features-in-C%23-6
    class CSharp6Features
    {
        async void Test()
        {
            // Using static
            WriteLine(Sqrt(3*3 + 4*4));
            WriteLine(Friday - Monday);            
            var range = Range(5, 17);                // Ok: not extension
            var even = range.Where(i => i % 2 == 0); // Ok
            
            // Null-conditional operators
            int? length = customers?.Length; // null if customers is null
            Customer first = customers?[0];  // null if customers is null            
            int length = customers?.Length ?? 0; // 0 if customers is null
            int? first = customers?[0]?.Orders?.Count();
            PropertyChanged?.Invoke(this, args);
        }
    }
}