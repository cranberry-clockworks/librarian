namespace Librarian.Api.Models;

public class UnknownDefinition : Definition
{
    public UnknownDefinition(int articleId, string lemma)
        : base(articleId, lemma) { }
}
