using System.Text.Json.Serialization;

namespace Librarian.No.Dictionaries.Client;

public class ArticleSearch
{
    [JsonPropertyName("bm")]
    public IReadOnlyCollection<int> Bookmaal { get; set; } = [];

    [JsonPropertyName("nn")]
    public IReadOnlyCollection<int> Nynorsk { get; set; } = [];
}
