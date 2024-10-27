using System.Text.Json.Serialization;

namespace Librarian.No.Dictionaries.Client;

public class Article
{
    [JsonPropertyName("article_id")]
    public int ArticleId { get; init; }

    [JsonPropertyName("lemmas")]
    public IReadOnlyCollection<Lemma> Lemmas { get; init; } = [];
}
