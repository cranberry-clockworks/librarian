using Librarian.Api.No;

namespace Librarian.Api.Models;

public class Definition
{
    public string WordClass { get; }
    public string Prefix { get; }
    public string Lemma { get; }
    public ICollection<Inflection> Inflections { get; }

    public Definition(
        string wordClass,
        string prefix,
        string lemma,
        ICollection<Inflection> inflections
    )
    {
        WordClass = wordClass;
        Prefix = prefix;
        Lemma = lemma;
        Inflections = inflections;
    }
}
