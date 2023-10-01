namespace Librarian.Api.Models;

public class NounDefinition : IDefinition
{
    public string Lemma { get; }

    public WordClass WordClass => WordClass.Noun;

    public string Article { get; }

    public NounDefinition(string lemma, string article)
    {
        Lemma = lemma;
        Article = article;
    }
}
