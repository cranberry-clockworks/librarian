using System.Diagnostics;

namespace Librarian.Api.No;

public class NorwegianDefinitionProvider
{
    private readonly Clients.OrdbokClient _client;
    private readonly ILogger<NorwegianDefinitionProvider> _logger;

    public NorwegianDefinitionProvider(
        ILogger<NorwegianDefinitionProvider> logger,
        Clients.OrdbokClient client
    )
    {
        _logger = logger;
        _client = client;
    }

    public async Task<ICollection<Models.IDefinition>> GetDefinitionsAsync(
        string word,
        int limit = 3,
        CancellationToken token = default
    )
    {
        var searchResult = await _client.SearchArticlesAsync(
            word,
            Clients.Dictionary.Bokmaal,
            Clients.WordClass.Any,
            Clients.Scope.ExactLemma | Clients.Scope.InflectedForms | Clients.Scope.FullTextSearch,
            token
        );

        Debug.Assert(searchResult.Bookmaal != null);

        var result = new List<Models.IDefinition>();

        foreach (var articleId in searchResult.Bookmaal.Take(limit))
        {
            var article = await _client.GetArticleAsync(
                Clients.Dictionary.Bokmaal,
                articleId,
                token
            );
            var definition = ToDefinition(article);
            if (definition != null)
            {
                result.Add(definition);
            }
        }

        return result;
    }

    private Models.IDefinition? ToDefinition(Clients.Article article)
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
            "m1" => new Models.NounDefinition(article.ArticleId, lemma.Value, Grammar.Article.Male),
            "f1"
                => new Models.NounDefinition(
                    article.ArticleId,
                    lemma.Value,
                    Grammar.Article.Female
                ),
            "n1"
                => new Models.NounDefinition(
                    article.ArticleId,
                    lemma.Value,
                    Grammar.Article.Neutral
                ),
            "verb"
                => new Models.VerbDefinition(
                    article.ArticleId,
                    lemma.Value,
                    ToInflectionModel(lemma.Paradigms.First().Inflections)
                ),
            _ => new Models.UnknownDefinition(article.ArticleId, lemma.Value),
        };
    }

    private static ICollection<Models.Inflection> ToInflectionModel(
        IEnumerable<Clients.Inflection> inflections
    )
    {
        var result = new List<Models.Inflection>();

        foreach (var inflection in inflections)
        {
            result.AddRange(
                inflection.Tags.Select(tag => new Models.Inflection(tag, inflection.WordForm))
            );
        }

        return result;
    }
}
