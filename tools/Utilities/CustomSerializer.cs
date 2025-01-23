using System.Text.Json.Serialization;
using Octokit;

namespace Utilities;

// This defines the custom serializer for the annotations block.
// That enables the status checks to write the annotations to
// the GitHub Actions output. That lets a subsequent action read
// these annotations in a different security context and write
// them to the PR.
[JsonSerializable(typeof(IList<NewCheckRunAnnotation>))]
public sealed partial class JsonCheckRunAnnotationSerializerContext : JsonSerializerContext;
