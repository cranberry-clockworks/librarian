namespace Librarian.Api.Models;

public class VerbDefinition : Definition
{
    public ICollection<Inflection> Inflections { get; }

    public VerbDefinition(int articleId, string lemma, ICollection<Inflection> inflections)
        : base(articleId, lemma)
    {
        Inflections = inflections;
    }
}

public record Inflection(string Type, string Word);
