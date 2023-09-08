using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;

namespace Librarian.Api.No;

public class NorwegianDefinitionProvider
{
    // private readonly ILogger _logger;
    private readonly IHttpClientFactory _factory;

    public NorwegianDefinitionProvider(IHttpClientFactory factory)
    {
        // _logger = logger;
        _factory = factory;
    }
    
    public async Task<string> GetDefinitionAsync(string lemma, CancellationToken token)
    {
        return "test";
    }
}