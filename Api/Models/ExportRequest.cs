namespace Librarian.Api.Models;

public class ExportRequest
{
    public string Deck { get; set; } = string.Empty;
    public IEnumerable<Card> Cards { get; set; } = Array.Empty<Card>();
}
