using Microsoft.AspNetCore.Mvc;

namespace Librarian.Cards.Anki;

[Route("anki")]
public class Controller(IAnkiService service, ITemplateRenderer renderer)
    : Microsoft.AspNetCore.Mvc.Controller
{
    [HttpPost]
    public async Task<IActionResult> ExportAsync([FromForm] string deck, CancellationToken token)
    {
        var cards = await renderer.RenderAsync(HttpContext.Session.GetCards().Values, token);
        await service.AddCards(deck, cards, token);
        return PartialView("_Notification", "Cards were successfully exported.");
    }
}
