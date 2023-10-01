namespace Librarian.Api.Models;

public interface IDefinition
{
    public int ArticleId { get; }

    public string Lemma { get; }

    public string Class { get; }
}
