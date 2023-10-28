namespace Librarian.Api.Models;

public class Card
{
    public string Front { get; set; } = string.Empty;
    public string Back { get; set; } = string.Empty;
    public IReadOnlyDictionary<string, string> Media { get; set; } =
        new Dictionary<string, string>();
}
