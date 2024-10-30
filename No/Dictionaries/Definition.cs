namespace Librarian.No.Dictionaries;

public class Definition
{
    public required DateOnly Entry { get; init; }
    public required string PartOfSpeech { get; init; }
    public required IReadOnlyList<Inflection> Inflections { get; init; }
}

public class Inflection
{
    public required string Phrase { get; init; }
    public required IReadOnlyList<string> Tags { get; init; }
}
