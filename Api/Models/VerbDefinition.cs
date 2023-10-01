namespace Librarian.Api.Models;

public class VerbDefinition : IDefinition
{
    public int ArticleId { get; }
    public string Lemma { get; }

    public ICollection<Inflection> Inflections { get; }

    public string Class => WordClass.Verb;

    public VerbDefinition(int articleId, string lemma, ICollection<Inflection> inflections)
    {
        ArticleId = articleId;
        Lemma = lemma;
        Inflections = inflections;
    }
}

public record Inflection(string Type, string Word);
