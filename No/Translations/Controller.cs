using Microsoft.AspNetCore.Mvc;

namespace Librarian.No.Translations;

[Route("/no/translations")]
public class Controller(IService service) : Microsoft.AspNetCore.Mvc.Controller
{
    [HttpGet("{phrase}")]
    public async Task<IActionResult> TranslateAsync(
        [FromRoute] string phrase,
        CancellationToken token
    )
    {
        var translation = await service.TranslateAsync(phrase, token);
        return PartialView("_Translation", translation);
    }
}
