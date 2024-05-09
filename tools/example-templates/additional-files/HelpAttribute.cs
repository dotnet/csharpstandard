[AttributeUsage(AttributeTargets.Class)]
public class HelpAttribute : Attribute
{
    public HelpAttribute(string url) // url is a positional parameter
    { 
        Url = url;
    }

    // Topic is a named parameter
    public string Topic
    { 
        get;
        set;
    } = null!;

    public string Url { get; } = null!;
}
