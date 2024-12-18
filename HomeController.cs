using System.Net.Mime;
using System.Net.Sockets;
using Librarian.Cards;
using Librarian.Cards.Anki;
using Librarian.No.Dictionaries;
using Microsoft.AspNetCore.Mvc;
using Card = Librarian.Cards.Card;
using IDictionaryService = Librarian.No.Dictionaries.IService;
using ITranslationService = Librarian.No.Translations.IService;

namespace Librarian;

[ApiController]
[Route("/")]
public class HomeController(
    ILogger<HomeController> logger,
    IDictionaryService dictionaryService,
    ITranslationService translationService,
    IAnkiConnect connect
) : Microsoft.AspNetCore.Mvc.Controller
{
    [HttpGet]
    public IActionResult GetIndex()
    {
        ViewBag.Cards = HttpContext.Session.GetCards();
        return View("Index");
    }

    private const int PageSize = 3;
    
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string phrase,
        [FromQuery] PartOfSpeech pos,
        CancellationToken token
    )
    {
        var articles = (await dictionaryService.GetArticlesAsync(phrase, pos, token)).Take(PageSize);
        var definitions = await dictionaryService.GetDefinitionsAsync(articles, token);
        ViewBag.Cards = HttpContext.Session.GetCards();
        ViewBag.Search = phrase;
        ViewBag.Phrase = phrase;
        ViewBag.PartOfSpeech = pos.ToString();
        ViewBag.NextPage = 1;
        return View("Search", definitions);
    }
    
    [HttpGet("search/more")]
    public async Task<IActionResult> Search(
        [FromQuery] string phrase,
        [FromQuery] PartOfSpeech pos,
        [FromQuery] int page,
        CancellationToken token
    )
    {
        var articles = (await dictionaryService.GetArticlesAsync(phrase, pos, token)).Skip(page * PageSize).Take(PageSize);
        var definitions = await dictionaryService.GetDefinitionsAsync(articles, token);
        ViewBag.Cards = HttpContext.Session.GetCards();
        ViewBag.Phrase = phrase;
        ViewBag.PartOfSpeech = pos.ToString();
        ViewBag.NextPage = page + 1;
        return PartialView("_SearchPage", definitions);
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

        ViewBag.Cards = HttpContext.Session.GetCards();
        return View("Export", decks);
    }
    
    
    [HttpGet("edit/{cardId:int}")]
    [Produces(MediaTypeNames.Text.Html)]
    public async Task<IActionResult> GetCardAsync([FromRoute] int cardId, CancellationToken token)
    {
        var cards = HttpContext.Session.GetCards();
        if (!cards.TryGetValue(cardId, out var card))
        {
            return NotFound($"Card with id '{cardId}' is not found.");
        }

        if (card.Translation == null)
        {
            card = new Card
            {
                Phrase = card.Phrase,
                Tags = card.Tags,
                PartOfSpeech = card.PartOfSpeech,
                Translation = await translationService.TranslateAsync(card.Phrase, token)
            };

            cards[card.Id] = card;
            
            HttpContext.Session.SetCards(cards);
        }

        ViewBag.Cards = cards;
        return View("Edit", card);
    }
}
