using ExampleExtractor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

if (args.Length != 1)
{
    Console.WriteLine("Arguments: <markdown-directory>");
    return 1;
}

foreach (var markdownFile in Directory.GetFiles(args[0]))
{
    Reformat(markdownFile);
}

return 0;

void Reformat(string file)
{
    const string ExamplePrefix = "<!-- Example: ";
    const string ExampleSuffix = " -->";

    Console.WriteLine($"Reformatting {Path.GetFileName(file)}");
    var lines = File.ReadAllLines(file);
    int changes = 0;

    for (int i = 0; i < lines.Length; i++)
    {
        string line = lines[i];

        int prefixIndex = line.IndexOf(ExamplePrefix);
        if (prefixIndex == -1)
        {
            continue;
        }
        if (!line.EndsWith(ExampleSuffix))
        {
            Console.WriteLine($"  WARNING: '{line}' does not end with {ExampleSuffix}");
        }
        string json = line[(prefixIndex + ExamplePrefix.Length)..^ExampleSuffix.Length];
        var metadata = JsonConvert.DeserializeObject<ExampleMetadata>(json)!;

        var reformatted = FormatMetadata(metadata);
        if (json == reformatted)
        {
            continue;
        }
        changes++;
        lines[i] = line[0..(prefixIndex + ExamplePrefix.Length)] + reformatted + ExampleSuffix;
    }

    if (changes != 0)
    {
        Console.WriteLine($"  Lines changed: {changes}");
        File.WriteAllLines(file, lines);
    }

    // Reformats the metadata to "not quite JSON" for brevity:
    // - No quotes around property names
    // - No space after opening { or before closing }
    // - No space after property colon, e.g. name:"value"
    // - Space between each property
    string FormatMetadata(ExampleMetadata metadata)
    {
        var settings = new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore };
        string plainJson = JsonConvert.SerializeObject(metadata, settings);
        JObject reparsed = JObject.Parse(plainJson);
        StringBuilder builder = new StringBuilder("{");
        bool first = true;
        foreach (var property in reparsed.Properties())
        {
            if (!first)
            {
                builder.Append(", ");
            }
            first = false;
            builder.Append(property.Name).Append(":").Append(SerializeValue(property.Value));
        }
        builder.Append("}");
        return builder.ToString();

        string SerializeValue(JToken token)
        {
            var stringWriter = new StringWriter();
            var jsonWriter = new JsonTextWriter(stringWriter);
            JsonSerializer.CreateDefault().Serialize(jsonWriter, token);
            return stringWriter.ToString();
        }
    }
}
