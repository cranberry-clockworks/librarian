using System.Text.Json;

namespace Librarian.Cards;

/// <summary>
/// Extension of <see cref="ISession"/>.
/// </summary>
public static class SessionExtension
{
    private const string CardsSessionKey = "Cards";

    /// <summary>
    /// Gets cards stored in the session.
    /// </summary>
    /// <returns>
    /// Dictionary of cards where key is a card ID and values are cards themselves.
    /// </returns>
    public static Dictionary<int, Card> GetCards(this ISession session)
    {
        var serialized = session.GetString(CardsSessionKey);
        if (string.IsNullOrWhiteSpace(serialized))
        {
            return new Dictionary<int, Card>();
        }

        try
        {
            var cards = JsonSerializer.Deserialize<Dictionary<int, Card>>(serialized);
            return cards ?? new Dictionary<int, Card>();
        }
        catch (JsonException)
        {
            return new Dictionary<int, Card>();
        }
    }

    /// <summary>
    /// Stores cards in the session.
    /// </summary>
    /// <param name="session">
    /// HTTP session.
    /// </param>
    /// <param name="cards">
    /// Cards to store. The key is a card ID and values are card themselves. 
    /// </param>
    public static void SetCards(this ISession session, IReadOnlyDictionary<int, Card> cards)
    {
        var serialized = JsonSerializer.Serialize(cards);
        session.SetString(CardsSessionKey, serialized);
    }
}
