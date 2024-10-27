using Microsoft.AspNetCore.Mvc;

namespace Librarian.No.Dictionaries;

[ApiController]
[Route("/no/dictionary")]
public class Controller(IService service) : Microsoft.AspNetCore.Mvc.Controller
{
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string phrase,
        [FromQuery] PartOfSpeech pos,
        CancellationToken token
    )
    {
        var definitions = await service.GetDefinitionsAsync(phrase, pos, token);
        return View("_SearchResult", definitions);
    }
}
