using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Librarian.No.Dictionaries.Client;

public interface IOrdbokClient
{
    Task<ArticleSearch> SearchArticlesAsync(
        string word,
        Dictionary dictionaries,
        WordClass wordClass,
        Scope scope,
        CancellationToken token
    );

    Task<Article> GetArticleAsync(Dictionary dictionary, int id, CancellationToken token);
}

internal class OrdbokClient(HttpClient client) : IOrdbokClient
{
    private static readonly JsonSerializerOptions DeserializationOptions = new();

    public async Task<ArticleSearch> SearchArticlesAsync(
        string word,
        Dictionary dictionaries,
        WordClass wordClass,
        Scope scope,
        CancellationToken token
    )
    {
        var uri = UriExtension.BuildUriWithQueryParameters(
            "/api/articles",
            BuildArticleSearchQueryParameters(word, dictionaries, wordClass, scope)
        );

        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json)
        );

        using var response = await client.SendAsync(request, token);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync(token);
            throw new ApiException(
                $"The HTTP status code of the response was not expected ({response.StatusCode})",
                response.StatusCode,
                content,
                response.Headers
            );
        }

        await using var stream = await response.Content.ReadAsStreamAsync(token);
        var result = await JsonSerializer.DeserializeAsync<ArticleSearchResponse>(
            stream,
            DeserializationOptions,
            token
        );

        Debug.Assert(result != null);

        return result.Articles;
    }

    public async Task<Article> GetArticleAsync(
        Dictionary dictionary,
        int id,
        CancellationToken token
    )
    {
        Debug.Assert(dictionary != Dictionary.None);
        Debug.Assert(dictionary != (Dictionary.Bokmaal | Dictionary.Nynorsk));

        var uri = $"/{(dictionary == Dictionary.Bokmaal ? "bm" : "nn")}/article/{id}.json";

        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json)
        );

        using var response = await client.SendAsync(request, token);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync(token);
            throw new ApiException(
                $"The HTTP status code of the response was not expected ({response.StatusCode})",
                response.StatusCode,
                content,
                response.Headers
            );
        }

        await using var stream = await response.Content.ReadAsStreamAsync(token);
        var result = await JsonSerializer.DeserializeAsync<Article>(
            stream,
            DeserializationOptions,
            token
        );

        Debug.Assert(result != null);

        return result;
    }

    private static IEnumerable<(string, string?)> BuildArticleSearchQueryParameters(
        string word,
        Dictionary dictionaries,
        WordClass wordClass,
        Scope scope
    )
    {
        yield return ("w", Uri.EscapeDataString(word));
        yield return ("dict", DictionariesToQueryParameterValue(dictionaries));
        yield return ("wc", WordClassToQueryParameterValue(wordClass));
        yield return ("scope", ScopeToQueryParameterValue(scope));
    }

    private static string? DictionariesToQueryParameterValue(Dictionary dictionarieses)
    {
        if (dictionarieses == Dictionary.None)
            return null;

        var list = new List<string>(2);
        if ((dictionarieses & Dictionary.Bokmaal) == Dictionary.Bokmaal)
            list.Add("bm");

        if ((dictionarieses & Dictionary.Nynorsk) == Dictionary.Nynorsk)
            list.Add("nn");

        return string.Join(",", list);
    }

    private static string? WordClassToQueryParameterValue(WordClass wordClass)
    {
        return wordClass switch
        {
            WordClass.Any => null,
            WordClass.Adjective => "ADJ",
            WordClass.Adposition => "ADP",
            WordClass.Adverb => "ADV",
            WordClass.Auxiliary => "AUX",
            WordClass.CoordinatingConjunction => "CCONJ",
            WordClass.Determiner => "DET",
            WordClass.Interjection => "INTJ",
            WordClass.Noun => "NOUN",
            WordClass.Numeral => "NUM",
            WordClass.Particle => "PART",
            WordClass.Pronoun => "PRON",
            WordClass.ProperNoun => "PPROPN",
            WordClass.Punctuation => "PUNCT",
            WordClass.SubordinatingConjunction => "SCONJ",
            WordClass.Symbol => "SYM",
            WordClass.Verb => "VERB",
            WordClass.Other => "X",
            _ => throw new ArgumentOutOfRangeException(nameof(wordClass), wordClass, null),
        };
    }

    private static string? ScopeToQueryParameterValue(Scope scope)
    {
        if (scope == Scope.None)
            return null;

        var builder = new StringBuilder();
        if ((scope & Scope.ExactLemma) == Scope.ExactLemma)
            builder.Append('e');

        if ((scope & Scope.InflectedForms) == Scope.InflectedForms)
            builder.Append('i');

        if ((scope & Scope.FullTextSearch) == Scope.FullTextSearch)
            builder.Append('f');

        return builder.ToString();
    }

    private class ArticleSearchResponse
    {
        [JsonPropertyName("articles")]
        public required ArticleSearch Articles { get; init; }
    }
}
