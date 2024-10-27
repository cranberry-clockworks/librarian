namespace Librarian.Cards.Anki;

public interface IAnkiService
{
    Task AddCards(string deck, IEnumerable<Card> cards, CancellationToken token);
}
