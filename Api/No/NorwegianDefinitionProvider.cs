using System.Diagnostics;
using Libraian.Api.Clients;

namespace Librarian.Api.No;

public class NorwegianDefinitionProvider
{
    private readonly ILogger<NorwegianDefinitionProvider> _logger;
    private readonly OrdbokClient _client;

    public NorwegianDefinitionProvider(ILogger<NorwegianDefinitionProvider> logger, OrdbokClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<string> GetDefinitionsAsync(string word)
    {
        _logger.LogInformation("Begin");
        var response = await _client.ArticlesAsync(word, "bm", "", "eif");
        _logger.LogInformation("Result {Result}", response.Articles.Bm.Count);
        return string.Empty;
    }
}