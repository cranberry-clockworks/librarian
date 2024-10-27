using System.Text.Json.Serialization;

namespace Librarian.No.Dictionaries.Client;

public class ArticleSearch
{
    [JsonPropertyName("bm")]
    public ICollection<int> Bookmaal { get; set; } = [];

    [JsonPropertyName("nn")]
    public ICollection<int> Nynorsk { get; set; } = [];
}
