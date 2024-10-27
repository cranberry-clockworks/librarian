using System.Text.Json.Serialization;

namespace Librarian.No.Dictionaries.Client;

public class ParadigmInfo
{
    [JsonPropertyName("from")]
    public DateOnly From { get; set; }

    [JsonPropertyName("inflection_group")]
    public string InflectionGroup { get; set; } = string.Empty;

    [JsonPropertyName("inflection")]
    public ICollection<Inflection> Inflections { get; set; } = new List<Inflection>();

    [JsonPropertyName("tags")]
    public ICollection<string> Tags { get; set; } = new List<string>();
}
