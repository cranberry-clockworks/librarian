namespace Librarian.Api.Models;

public class NounDefinition : Definition
{
    public string Article { get; }

    public NounDefinition(int articleId, string lemma, string article)
        : base(articleId, lemma)
    {
        Article = article;
    }
}
