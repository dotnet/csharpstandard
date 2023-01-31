using System;
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AuthorAttribute : Attribute
{
     public string Name { get; }
     public AuthorAttribute(string name) => Name = name;
}
