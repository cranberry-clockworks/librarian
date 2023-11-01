using System.Diagnostics;
using Librarian.Api.Models.Definitions;

namespace Librarian.Api.No.Definitions;

public class DefinitionService
{
    private readonly OrdbokClient _client;
    private readonly ILogger<DefinitionService> _logger;

    public DefinitionService(ILogger<DefinitionService> logger, OrdbokClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<ICollection<Definition>> GetDefinitionsAsync(
        string word,
        string? wordClass,
        int limit = 3,
        CancellationToken token = default
    )
    {
        var searchResult = await _client.SearchArticlesAsync(
            word,
            Dictionary.Bokmaal,
            TryConvertWordClass(wordClass),
            Scope.ExactLemma | Scope.InflectedForms | Scope.FullTextSearch,
            token
        );

        Debug.Assert(searchResult.Bookmaal != null);

        var result = new List<Definition>();

        foreach (var articleId in searchResult.Bookmaal.Take(limit))
        {
            var article = await _client.GetArticleAsync(Dictionary.Bokmaal, articleId, token);
            var definition = ToDefinition(article);
            if (definition != null)
            {
                result.Add(definition);
            }
        }

        if (result.Count == 0)
        {
            result.Add(new Phrase(word));
        }

        return result;
    }

    private Definition? ToDefinition(Article article)
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
            "m1" => new Noun(lemma.Value, Grammar.Article.Male),
            "f1" => new Noun(lemma.Value, Grammar.Article.Female),
            "n1" => new Noun(lemma.Value, Grammar.Article.Neutral),
            "v1"
            or "verb"
                => new Verb(lemma.Value, ToInflectionModel(lemma.Paradigms.First().Inflections)),
            "a1" => new Adjective(lemma.Value),
            _ => new Phrase(lemma.Value),
        };
    }

    private static ICollection<Models.Definitions.Inflection> ToInflectionModel(
        IEnumerable<Inflection> inflections
    )
    {
        var result = new List<Models.Definitions.Inflection>();

        foreach (var inflection in inflections)
        {
            result.AddRange(
                inflection.Tags.Select(
                    tag => new Models.Definitions.Inflection(tag, inflection.WordForm)
                )
            );
        }

        return result.DistinctBy(static x => $"{x.Type}{x.Word}").ToList();
    }

    private static WordClass TryConvertWordClass(string? wordClass)
    {
        if (string.IsNullOrEmpty(wordClass))
        {
            return WordClass.Any;
        }

        if (wordClass.Equals(Models.WordClass.Noun, StringComparison.OrdinalIgnoreCase))
        {
            return WordClass.Noun;
        }

        if (wordClass.Equals(Models.WordClass.Verb, StringComparison.OrdinalIgnoreCase))
        {
            return WordClass.Verb;
        }

        if (wordClass.Equals(Models.WordClass.Adjective, StringComparison.OrdinalIgnoreCase))
        {
            return WordClass.Adjective;
        }

        throw new ArgumentException("Unknown word class", nameof(wordClass));
    }
}
