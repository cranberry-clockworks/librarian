namespace Librarian.Api.Models;

public class UnknownDefinition : IDefinition
{
    public string Lemma { get; }
    public WordClass WordClass => WordClass.Unknown;

    public UnknownDefinition(string lemma)
    {
        Lemma = lemma;
    }
}
