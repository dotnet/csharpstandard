namespace Comments.XmlComments.UndocumentedKeywords
{
    // From here:https://github.com/dotnet/roslyn/wiki/New-Language-Features-in-C%23-6
    class CSharp6Features
    {
        async void Test()
        {
            // String interpolation
            string s = $"{p.Name, 20} is {p.Age:D3} year{{s}} old #";
            s = $"{p.Name} is \"{p.Age} year{(p.Age == 1 ? "" : "s")} old";
            s = $"{(p.Age == 2 ? $"{new Person { } }" : "")}";
            s = $@"\{p.Name}
                                   ""\";
            s = $"Color [ R={func(b: 3):#0.##}, G={G:#0.##}, B={B:#0.##}, A={A:#0.##} ]";
            
            // nameof expressions
            if (x == null)
                throw new ArgumentNullException(nameof(x));
            WriteLine(nameof(person.Address.ZipCode)); // prints "ZipCode"
        }
    }
}