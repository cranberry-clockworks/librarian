namespace Librarian.Api.Models;

public class UnknownDefinition : IDefinition
{
    public int ArticleId { get; }
    public string Lemma { get; }
    public string Class => WordClass.Unknown;

    public UnknownDefinition(int articleId, string lemma)
    {
        ArticleId = articleId;
        Lemma = lemma;
    }
}
