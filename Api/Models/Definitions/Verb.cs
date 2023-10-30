namespace Librarian.Api.Models.Definitions;

public class Verb : Definition
{
    public ICollection<Inflection> Inflections { get; }

    public Verb(string lemma, ICollection<Inflection> inflections)
        : base(lemma)
    {
        Inflections = inflections;
    }
}

public record Inflection(string Type, string Word);
