using Microsoft.AspNetCore.Mvc;

namespace Librarian.No.Pronunciations;

[ApiController]
[Route("/no/pronunciations")]
public class Controller(IService service) : Microsoft.AspNetCore.Mvc.Controller
{
    [HttpGet("{phrase}")]
    public async Task<IActionResult> GetAsync([FromRoute] string phrase, CancellationToken token)
    {
        var (contentType, content) = await service.PronounceAsync(phrase, token);
        return File(content, contentType);
    }
}
