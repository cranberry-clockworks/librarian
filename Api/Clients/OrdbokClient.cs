using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Librarian.Api.Clients;

public class OrdbokClient
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions DeserializationOptions = new();

    public OrdbokClient(HttpClient client)
    {
        _client = client;
        _client.BaseAddress = new Uri("https://ord.uib.no");
    }

    public async Task<ArticleSearch> SearchArticlesAsync(string word, Dictionary dictionarieses,
        WordClass wordClass, Scope scope, CancellationToken token)
    {
        var uri = UriExtension.BuildUriWithQueryParameters("/api/articles",
            BuildArticleSearchQueryParameters(word, dictionarieses, wordClass, scope));
        
        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
        
        using var response = await _client.SendAsync(request, token);
        
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync(token);
            throw new ApiException($"The HTTP status code of the response was not expected ({response.StatusCode})",
                response.StatusCode, content, response.Headers);
        }

        await using var stream = await response.Content.ReadAsStreamAsync(token);
        var result = await JsonSerializer.DeserializeAsync<ArticleSearchResponse>(stream, DeserializationOptions, token);
        
        Debug.Assert(result != null);
        
        return result.Articles;
    }

    public async Task<Article> GetArticleAsync(Dictionary dictionary, int id, CancellationToken token)
    {
        Debug.Assert(dictionary != Dictionary.None);
        Debug.Assert(dictionary != (Dictionary.Bokmaal | Dictionary.Nynorsk));
        
        var uri = $"/{(dictionary == Dictionary.Bokmaal ? "bm" : "nn")}/article/{id}.json";

        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
        
        using var response = await _client.SendAsync(request, token);
        
        if (response.StatusCode != HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync(token);
            throw new ApiException($"The HTTP status code of the response was not expected ({response.StatusCode})",
                response.StatusCode, content, response.Headers);
        }

        await using var stream = await response.Content.ReadAsStreamAsync(token);
        var result = await JsonSerializer.DeserializeAsync<Article>(stream, DeserializationOptions, token);
        
        Debug.Assert(result != null);
        
        return result;
    }

    private IEnumerable<(string, string?)> BuildArticleSearchQueryParameters(
        string word, Dictionary dictionarieses, WordClass wordClass, Scope scope)
    {
        yield return ("w", Uri.EscapeDataString(word));
        yield return ("dict", DictionariesToQueryParameterValue(dictionarieses));
        yield return ("wc", WordClassToQueryParameterValue(wordClass));
        yield return ("scope", ScopeToQueryParameterValue(scope));
    }

    private static string? DictionariesToQueryParameterValue(Dictionary dictionarieses)
    {
        if (dictionarieses == Dictionary.None) return null;

        var list = new List<string>(2);
        if ((dictionarieses & Dictionary.Bokmaal) == Dictionary.Bokmaal)
        {
            list.Add("bm");
        }

        if ((dictionarieses & Dictionary.Nynorsk) == Dictionary.Nynorsk)
        {
            list.Add("nn");
        }

        return string.Join(",", list);
    }

    private static string? WordClassToQueryParameterValue(WordClass wordClass) =>
        wordClass switch
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
            _ => throw new ArgumentOutOfRangeException(nameof(wordClass), wordClass, null)
        };

    private static string? ScopeToQueryParameterValue(Scope scope)
    {
        if (scope == Scope.None)
        {
            return null;
        }

        var builder = new StringBuilder();
        if ((scope & Scope.ExactLemma) == Scope.ExactLemma)
        {
            builder.Append('e');
        }

        if ((scope & Scope.InflectedForms) == Scope.InflectedForms)
        {
            builder.Append('i');
        }

        if ((scope & Scope.FullTextSearch) == Scope.FullTextSearch)
        {
            builder.Append('f');
        }

        return builder.ToString();
    }

    private class ArticleSearchResponse
    {
        [JsonPropertyName("articles")]
        public ArticleSearch Articles { get; init; } = new();
    }
}

[Flags]
public enum Dictionary
{
    None = 0,
    Bokmaal = 1,
    Nynorsk = 2
};

public enum WordClass
{
    Any,
    Adjective,
    Adposition,
    Adverb,
    Auxiliary,
    CoordinatingConjunction,
    Determiner,
    Interjection,
    Noun,
    Numeral,
    Particle,
    Pronoun,
    ProperNoun,
    Punctuation,
    SubordinatingConjunction,
    Symbol,
    Verb,
    Other
}

[Flags]
public enum Scope
{
    None = 0,
    ExactLemma = 1,
    FullTextSearch = 2,
    InflectedForms = 4,
}

public class ArticleSearch
{
    [JsonPropertyName("bm")]
    public ICollection<int>? Bookmaal { get; set; }
    
    [JsonPropertyName("nn")]
    public ICollection<int>? Nynorsk { get; set; }
}

public class Article
{
    [JsonPropertyName("article_id")]
    public int ArticleId { get; set; }
    
    [JsonPropertyName("lemmas")]
    public ICollection<Lemma> Lemmas { get; set; } = new List<Lemma>();
}


public class Lemma
{
    [JsonPropertyName("inflection_class")] public string InflectionClass { get; set; } = string.Empty;

    [JsonPropertyName("paradigm_info")]
    public ICollection<ParadigmInfo> Paradigms { get; set; } = new List<ParadigmInfo>();
}

public class ParadigmInfo
{
    [JsonPropertyName("inflection_group")]
    public string InflectionGroup { get; set; } = string.Empty;

    [JsonPropertyName("inflection")] public ICollection<Inflection> Inflections { get; set; } = new List<Inflection>();
    
    [JsonPropertyName("tags")] public ICollection<string> Tags { get; set; } = new List<string>();
}


public class Inflection
{
    [JsonPropertyName("word_form")] public string WordForm { get; set; } = string.Empty;
    
    [JsonPropertyName("tags")] public ICollection<string> Tags { get; set; } = new List<string>();
}