using System.Diagnostics;
using System.Text.Json;
using Librarian.Api.Clients;

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
        var res = await _client.SearchArticlesAsync(word, Dictionary.Bokmaal, WordClass.Any,
            Scope.ExactLemma | Scope.InflectedForms | Scope.FullTextSearch, CancellationToken.None);
        
        Debug.Assert(res.Bookmaal != null);

        if (res.Bookmaal.Count == 0)
            return "NOT FOUND";

        var id = res.Bookmaal.First();

        var article = await _client.GetArticleAsync(Dictionary.Bokmaal, id, CancellationToken.None);

        return JsonSerializer.Serialize(article);
    }
}