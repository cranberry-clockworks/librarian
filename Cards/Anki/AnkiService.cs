namespace Librarian.Cards.Anki;

public class AnkiService(IAnkiConnect client) : IAnkiService
{
    public async Task AddCards(string deck, IEnumerable<Card> cards, CancellationToken token)
    {
        foreach (var card in cards)
        {
            await AddCard(deck, card, token);
        }

        await client.SyncAsync(token);
    }

    private async Task AddCard(string deck, Card card, CancellationToken token)
    {
        await AddMediaAsync(card.Media, token);
        await client.AddNoteAsync(deck, card.Front, card.Back, token);
    }

    private Task AddMediaAsync(
        IReadOnlyDictionary<string, string> media,
        CancellationToken token
    ) => Task.WhenAll(media.Select(kvp => AddMediaFileAsync(kvp.Key, kvp.Value, token)));

    private Task AddMediaFileAsync(string name, string content, CancellationToken token)
    {
        return client.StoreMediaFileAsync(name, content, token);
    }
}
