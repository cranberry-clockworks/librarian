using Microsoft.AspNetCore.Mvc;

namespace Librarian.Cards;

[ApiController]
[Route("cards")]
public class Controller : Microsoft.AspNetCore.Mvc.Controller
{
    [HttpDelete("{cardId:int}")]
    public IActionResult Delete([FromRoute] int cardId)
    {
        var cards = HttpContext.Session.GetCards();

        if (!cards.Remove(cardId))
        {
            return NotFound($"Card with id '{cardId}' is not found.");
        }

        HttpContext.Session.SetCards(cards);
        return Ok();
    }

    [HttpDelete]
    public IActionResult DeleteAll()
    {
        HttpContext.Session.SetCards(new Dictionary<int, Card>());
        return Ok();
    }

    [HttpPost]
    public IActionResult Add([FromForm] Card card)
    {
        var cards = HttpContext.Session.GetCards();

        if (!cards.TryAdd(card.Id, card))
        {
            return Conflict("Card is already exists.");
        }

        HttpContext.Session.SetCards(cards);
        return PartialView("_Card", new KeyValuePair<int, Card>(card.Id, card));
    }
}
