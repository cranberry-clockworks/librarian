namespace Librarian.Api.Models;

public interface IDefinition
{
    public string Lemma { get; }

    public WordClass WordClass { get; }
}
