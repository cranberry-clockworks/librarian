namespace Librarian.Cards.Anki;

public class Card
{
    public string Front { get; init; } = string.Empty;
    public string Back { get; init; } = string.Empty;

    /// <summary>
    /// Name -> Content in b64
    /// </summary>
    public IReadOnlyDictionary<string, string> Media { get; init; } =
        new Dictionary<string, string>();
}
