using Librarian.No.Dictionaries.Client;

namespace Librarian.No.Dictionaries;

public class Service(ILogger<Service> logger, IOrdbokClient client) : IService
{
    public async Task<IReadOnlyCollection<int>> GetArticlesAsync(string phrase, PartOfSpeech partOfSpeech, CancellationToken token)
    {
        var searchResult = await client.SearchArticlesAsync(
            phrase,
            Dictionary.Bokmaal,
            ToWordClass(partOfSpeech),
            Scope.ExactLemma | Scope.InflectedForms | Scope.FullTextSearch,
            token
        );

        return searchResult.Bookmaal;
    }
    public async Task<IReadOnlyCollection<Definition>> GetDefinitionsAsync(
        IEnumerable<int> articleIds,
        CancellationToken token
    )
    {
        var result = new List<Definition>();

        foreach (var articleId in articleIds)
        {
            var article = await client.GetArticleAsync(Dictionary.Bokmaal, articleId, token);
            var definition = TryConvertToDefinition(article);
            if (definition != null)
            {
                result.Add(definition);
            }
        }

        return result;
    }

    private static WordClass ToWordClass(PartOfSpeech partOfSpeech)
    {
        return partOfSpeech switch
        {
            PartOfSpeech.Any => WordClass.Any,
            PartOfSpeech.Noun => WordClass.Noun,
            PartOfSpeech.Verb => WordClass.Verb,
            PartOfSpeech.Adjective => WordClass.Adjective,
            _ => WordClass.Any,
        };
    }

    private Definition? TryConvertToDefinition(Article article)
    {
        var lemma = article.Lemmas.FirstOrDefault();
        if (lemma == null)
        {
            logger.LogWarning("No lemma in the article found");
            return null;
        }

        var latestEntry = lemma.Paradigms.FirstOrDefault();
        if (latestEntry == null)
        {
            logger.LogWarning("No paradigm found");
            return null;
        }

        var pos = ExtractDefinitionPartOfSpeech(latestEntry);
        var inflections = pos switch
        {
            "noun" => CreateInflectionsForNoun(lemma),
            "verb" => CreateInflectionsForVerb(latestEntry),
            _ => CreateInflectionsDefault(latestEntry),
        };

        return new Definition { PartOfSpeech = pos, Inflections = inflections };
    }

    private static string ExtractDefinitionPartOfSpeech(ParadigmInfo paradigm)
    {
        return paradigm.InflectionGroup.Split('_').First().ToLower();
    }

    private static List<Inflection> CreateInflectionsForNoun(Lemma lemma)
    {
        return [new Inflection { Phrase = GetPhrase(lemma), Tags = [] }];

        static string GetPhrase(Lemma lemma)
        {
            if (lemma.InflectionClass.StartsWith('n'))
            {
                return $"et {lemma.Value}";
            }

            if (lemma.InflectionClass.StartsWith('m'))
            {
                return $"en {lemma.Value}";
            }

            if (lemma.InflectionClass.StartsWith('f'))
            {
                return $"ei {lemma.Value}";
            }

            return $"{lemma.Value}";
        }
    }

    private static List<Inflection> CreateInflectionsForVerb(ParadigmInfo paradigm)
    {
        return paradigm
            .Inflections.Where(static x => !string.IsNullOrEmpty(x.WordForm))
            .Select(inflection => new Inflection
            {
                Phrase = GetPhrase(inflection),
                Tags = StripTags(inflection.Tags),
            })
            .ToList();

        static string GetPhrase(Dictionaries.Client.Inflection inflection)
        {
            var tags = inflection.Tags;

            if (tags.Count != 1)
            {
                return inflection.WordForm;
            }

            if (tags.Contains(Dictionaries.Client.Inflection.KnownTags.Verb.Infinitive))
            {
                return $"å {inflection.WordForm}";
            }

            if (
                tags.Contains(Dictionaries.Client.Inflection.KnownTags.Verb.PerfectParticiple)
                && tags.Count == 1
            )
            {
                return $"har {inflection.WordForm}";
            }

            return inflection.WordForm;
        }
    }

    private static List<Inflection> CreateInflectionsDefault(ParadigmInfo paradigm)
    {
        return paradigm
            .Inflections.Where(static x => !string.IsNullOrEmpty(x.WordForm))
            .Select(inflection => new Inflection
            {
                Phrase = inflection.WordForm,
                Tags = StripTags(inflection.Tags),
            })
            .ToList();
    }

    private static string[] StripTags(IEnumerable<string> tags)
    {
        return tags.Select(static t => t.Replace("<", "").Replace(">", "")).ToArray();
    }
}
