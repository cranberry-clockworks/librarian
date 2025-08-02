namespace Librarian.Api.Anki;

public class AnkiService
{
    private readonly ILogger<AnkiService> _logger;
    private readonly AnkiConnect _client;

    public AnkiService(ILogger<AnkiService> logger, AnkiConnect client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task AddCards(string deck, IEnumerable<Card> cards, CancellationToken token)
    {
        foreach (var card in cards)
        {
            await AddCard(deck, card, token);
        }

        await _client.SyncAsync(token);
    }

    private async Task AddCard(string deck, Card card, CancellationToken token)
    {
        await AddMediaAsync(card.Media, token);
        await _client.AddNoteAsync(deck, card.Front, card.Back, token);
    }

    private Task AddMediaAsync(
        IReadOnlyDictionary<string, string> media,
        CancellationToken token
    ) => Task.WhenAll(media.Select(kvp => AddMediaFileAsync(kvp.Key, kvp.Value, token)));

    private Task AddMediaFileAsync(string name, string content, CancellationToken token)
    {
        var data = StripMedia(content);
        return _client.StoreMediaFileAsync(name, data, token);
    }

    private static string StripMedia(string content) =>
        content.StartsWith(Media.FormatPrefix) ? content[Media.FormatPrefix.Length..] : content;
}
