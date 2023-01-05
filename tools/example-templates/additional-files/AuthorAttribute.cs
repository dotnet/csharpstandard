using System;
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AuthorAttribute : Attribute
{
    private string name;
    public AuthorAttribute(string name)
    {
        this.name = name;
    }
    public string Name
    {
        get { return name; }
    }
}
