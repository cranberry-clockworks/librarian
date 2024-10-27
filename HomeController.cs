using System.Net.Sockets;
using Librarian.Cards;
using Librarian.Cards.Anki;
using Librarian.No.Dictionaries;
using Microsoft.AspNetCore.Mvc;
using IDictionaryService = Librarian.No.Dictionaries.IService;

namespace Librarian;

[ApiController]
[Route("/")]
public class HomeController(
    ILogger<HomeController> logger,
    IDictionaryService service,
    IAnkiConnect connect
) : Microsoft.AspNetCore.Mvc.Controller
{
    [HttpGet]
    public IActionResult GetIndex()
    {
        ViewBag.Cards = HttpContext.Session.GetCards();
        return View("Index");
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string phrase,
        [FromQuery] PartOfSpeech pos,
        CancellationToken token
    )
    {
        var definitions = await service.GetDefinitionsAsync(phrase, pos, token);
        ViewBag.Cards = HttpContext.Session.GetCards();
        ViewBag.Search = phrase;
        return View("Search", definitions);
    }

    [HttpGet("export")]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        List<string> decks;
        try
        {
            decks = await connect.GetDecksAsync(cancellationToken);
        }
        catch (Exception e) when (e is ApiException or SocketException or HttpRequestException)
        {
            logger.LogError(e, "Failed to get decks");
            decks = [];
        }

        return View("Export", decks);
    }
}
