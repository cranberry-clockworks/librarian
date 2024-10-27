namespace Librarian.Cards.Anki;

public interface ITemplateRenderer
{
    Task<List<Card>> RenderAsync(IEnumerable<Cards.Card> templates, CancellationToken token);
}
