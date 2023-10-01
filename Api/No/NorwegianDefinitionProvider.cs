using System.Diagnostics;
using Librarian.Api.Clients;
using Librarian.Api.Models;

namespace Librarian.Api.No;

public class NorwegianDefinitionProvider
{
    private readonly OrdbokClient _client;
    private readonly ILogger<NorwegianDefinitionProvider> _logger;

    public NorwegianDefinitionProvider(
        ILogger<NorwegianDefinitionProvider> logger,
        OrdbokClient client
    )
    {
        _logger = logger;
        _client = client;
    }

    public async Task<ICollection<object>> GetDefinitionsAsync(
        string word,
        int limit = 3,
        CancellationToken token = default
    )
    {
        var searchResult = await _client.SearchArticlesAsync(
            word,
            Dictionary.Bokmaal,
            Clients.WordClass.Any,
            Scope.ExactLemma | Scope.InflectedForms | Scope.FullTextSearch,
            token
        );

        Debug.Assert(searchResult.Bookmaal != null);

        var result = new List<object>();

        foreach (var articleId in searchResult.Bookmaal.Take(limit))
        {
            var article = await _client.GetArticleAsync(Dictionary.Bokmaal, articleId, token);
            var definition = ToDefinition(article);
            if (definition != null)
            {
                result.Add(definition);
            }
        }

        return result;
    }

    private IDefinition? ToDefinition(Article article)
    {
        var lemma = article.Lemmas.FirstOrDefault();

        if (lemma == null)
        {
            _logger.LogWarning("Damaged structure of the article");
            return null;
        }

        var wordClass = lemma.InflectionClass;

        return wordClass switch
        {
            "m1" => new NounDefinition(lemma.Value, Grammar.Article.Male),
            "f1" => new NounDefinition(lemma.Value, Grammar.Article.Female),
            "n1" => new NounDefinition(lemma.Value, Grammar.Article.Neutral),
            "verb"
                => new VerbDefinition(
                    lemma.Value,
                    ToInflectionModel(lemma.Paradigms.First().Inflections)
                ),
            _ => new UnknownDefinition(lemma.Value),
        };
    }

    private static IDictionary<string, string> ToInflectionModel(
        IEnumerable<Inflection> inflections
    )
    {
        var result = new Dictionary<string, string>();

        foreach (var inflection in inflections)
        {
            foreach (var tag in inflection.Tags)
            {
                result[tag] = inflection.WordForm;
            }
        }

        return result;
    }
}
