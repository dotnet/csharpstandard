[AttributeUsage(AttributeTargets.Class)]
public class HelpAttribute : Attribute
{
    public HelpAttribute(string url) // url is a positional parameter
    { 
    }

    // Topic is a named parameter
    public string Topic
    { 
        get;
        set;
    }

    public string Url { get; }
}
