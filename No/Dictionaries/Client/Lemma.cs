using System.Text.Json.Serialization;

namespace Librarian.No.Dictionaries.Client;

public class Lemma
{
    [JsonPropertyName("lemma")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("inflection_class")]
    public string InflectionClass { get; set; } = string.Empty;

    [JsonPropertyName("paradigm_info")]
    public ICollection<ParadigmInfo> Paradigms { get; set; } = new List<ParadigmInfo>();
}
