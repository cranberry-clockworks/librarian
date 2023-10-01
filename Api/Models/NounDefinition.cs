namespace Librarian.Api.Models;

public class NounDefinition : IDefinition
{
    public int ArticleId { get; }
    public string Lemma { get; }

    public string Class => WordClass.Noun;

    public string Article { get; }

    public NounDefinition(int articleId, string lemma, string article)
    {
        ArticleId = articleId;
        Lemma = lemma;
        Article = article;
    }
}
