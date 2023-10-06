namespace Librarian.Api.Models;

public class UnknownDefinition : Definition
{
    public override string Class => WordClass.Unknown;

    public UnknownDefinition(int articleId, string lemma)
        : base(articleId, lemma) { }
}
