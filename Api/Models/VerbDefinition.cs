namespace Librarian.Api.Models;

public class VerbDefinition : IDefinition
{
    public string Lemma { get; }

    public IDictionary<string, string> Inflections { get; }

    public WordClass WordClass => WordClass.Verb;

    public VerbDefinition(string lemma, IDictionary<string, string> inflections)
    {
        Lemma = lemma;
        Inflections = inflections;
    }
}
