using System.Text.Json.Serialization;

namespace Librarian.No.Dictionaries.Client;

public class Inflection
{
    [JsonPropertyName("word_form")]
    public required string WordForm { get; init; } = string.Empty;

    [JsonPropertyName("tags")]
    public required IReadOnlyCollection<string> Tags { get; init; } = new List<string>();

    public static class KnownTags
    {
        public static class Verb
        {
            public const string Infinitive = "Inf";
            public const string PerfectParticiple = "<PerfPart>";
        }
    }
}
