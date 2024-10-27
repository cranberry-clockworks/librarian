using System.Text.Json;

namespace Librarian.Cards;

public static class SessionExtension
{
    private const string CardsSessionKey = "Cards";

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
        catch (JsonException e)
        {
            return new Dictionary<int, Card>();
        }
    }

    public static void SetCards(this ISession session, IReadOnlyDictionary<int, Card> cards)
    {
        var serialized = JsonSerializer.Serialize(cards);
        session.SetString(CardsSessionKey, serialized);
    }
}
