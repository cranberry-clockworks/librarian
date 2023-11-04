using System.Diagnostics;
using Librarian.Api.Models;

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

        result.Add(new Definition("phrase", "", word, Array.Empty<Models.Inflection>()));

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

        var wordClass = lemma.Paradigms
            .FirstOrDefault()
            ?.InflectionGroup.Split('_')
            .FirstOrDefault()
            ?.ToLower();

        return wordClass switch
        {
            "noun"
                => new Definition(
                    "noun",
                    GetArticleForNoun(lemma),
                    lemma.Value,
                    Array.Empty<Models.Inflection>()
                ),
            "verb"
                => new Definition(
                    "verb",
                    string.Empty,
                    lemma.Value,
                    ToInflectionModel(lemma.Paradigms.First().Inflections)
                ),
            _
                => new Definition(
                    wordClass ?? string.Empty,
                    string.Empty,
                    lemma.Value,
                    Array.Empty<Models.Inflection>()
                ),
        };
    }

    private static string GetArticleForNoun(Lemma lemma)
    {
        if (lemma.InflectionClass.Contains('n'))
        {
            return Grammar.Article.Neutral;
        }

        if (lemma.InflectionClass.Contains('f'))
        {
            return Grammar.Article.Female;
        }

        if (lemma.InflectionClass.Contains('m'))
        {
            return Grammar.Article.Male;
        }

        return string.Empty;
    }

    private static ICollection<Models.Inflection> ToInflectionModel(
        IEnumerable<Inflection> inflections
    )
    {
        var result = new List<Models.Inflection>();

        foreach (var inflection in inflections)
        {
            result.AddRange(
                inflection.Tags.Select(tag => new Models.Inflection(tag, inflection.WordForm))
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
