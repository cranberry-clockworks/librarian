using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace Librarian.Cards;

/// <summary>
/// A controller for managing export card collection.
/// </summary>
[Route("cards")]
public class Controller : Microsoft.AspNetCore.Mvc.Controller
{
    /// <summary>
    /// Deletes card from the export collection.
    /// </summary>
    /// <param name="cardId">
    /// The card ID.
    /// </param>
    [HttpDelete("{cardId:int}")]
    [Produces(MediaTypeNames.Text.Html)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Deletes all cards from the export collection.
    /// </summary>
    /// <returns></returns>
    [HttpDelete]
    [Produces(MediaTypeNames.Text.Html)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult DeleteAll()
    {
        HttpContext.Session.SetCards(new Dictionary<int, Card>());
        return Ok();
    }

    /// <summary>
    /// Adds the card to the export collection.
    /// </summary>
    /// <param name="card">
    /// The new card content.
    /// </param>
    [HttpPost]
    [Produces(MediaTypeNames.Text.Html)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public IActionResult Add([FromForm] Card card)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }
        
        var cards = HttpContext.Session.GetCards();

        if (!cards.TryAdd(card.Id, card))
        {
            return Conflict("Card is already exists.");
        }

        HttpContext.Session.SetCards(cards);
        return PartialView("_Card", new KeyValuePair<int, Card>(card.Id, card));
    }
}
