using System.Diagnostics;
using System.Net.Mime;
using System.Text.Json;
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
        return PartialView("_WordListEntry", new KeyValuePair<int, Card>(card.Id, card));
    }

    [HttpPatch("{cardId:int}")]
    public IActionResult PatchTranslation(
        [FromRoute] int cardId,
        [FromForm] string translation,
        [FromForm(Name = "example-en")] string enExample,
        [FromForm(Name = "example-no")] string noExample)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }
        
        var cards = HttpContext.Session.GetCards();

        if (!cards.TryGetValue(cardId, out var card))
        {
            return NotFound("Card is already exists.");
        }

        card = card with 
        {
            Translation = translation,
            EnglishUsageExample = enExample,
            NorwegianUsageExample = noExample
        };

        cards[card.Id] = card;
        
        HttpContext.Session.SetCards(cards);
        return PartialView("_Notification", "Card was successfully updated.");
    }

    [HttpPost("refinements")]
    public IActionResult Refinement([FromForm(Name = "prompt-response")] string response)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        IReadOnlyCollection<RefinementResponse> refinements;
        try
        {
            refinements = JsonSerializer.Deserialize<IReadOnlyCollection<RefinementResponse>>(response)!;
        }
        catch (Exception e) when( e is JsonException or NullReferenceException)
        {
            return BadRequest("Failed to process refinement response.");
        }

        var cards = HttpContext.Session.GetCards();
        foreach (var refinement in refinements)
        {
            if (!cards.TryGetValue(refinement.Id, out var card))
            {
                return BadRequest($"Card is not found with the given id: {refinement.Id}");
            }

            var newCard = card with
            {
                Translation = refinement.PhraseEnglish,
                NorwegianUsageExample = refinement.ExampleNorwegian,
                EnglishUsageExample = refinement.ExampleEnglish
            };
            
            Debug.Assert(card.Id == newCard.Id);
            
            cards[card.Id] = newCard;
        }
        
        HttpContext.Session.SetCards(cards);

        return PartialView("_Notification", "Cards were successfully updated.");
    }
}
