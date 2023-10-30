namespace Librarian.Api.Models.Definitions;

public class Noun : Definition
{
    public string Article { get; }

    public Noun(string lemma, string article)
        : base(lemma)
    {
        Article = article;
    }
}
