namespace Comments.XmlComments.UndocumentedKeywords
{
    // From here:https://github.com/dotnet/roslyn/wiki/New-Language-Features-in-C%23-6
    class CSharp6Features
    {
        // Initializers for auto-properties
        public string First { get; set; } = "Jane";
        public string Last { get; set; } = "Doe";
    
        // Getter-only auto-properties
        public string Third { get; } = "Jane";
        public string Fourth { get; } = "Doe";
        
        // Expression bodies on method-like members
        public Point Move(int dx, int dy) => new Point(x + dx, y + dy); 
        public static Complex operator +(Complex a, Complex b) => a.Add(b);
        public static implicit operator string(Person p) => p.First + " " + p.Last;
        public void Print() => Console.WriteLine(First + " " + Last);
        
        // Expression bodies on property-like function members
        public string Name => First + " " + Last;
        public int this[long id] => id;
    }
}