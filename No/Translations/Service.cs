using System.Diagnostics;
using DeepL;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Librarian.No.Translations;

public interface IService
{
    Task<string> TranslateAsync(string text, CancellationToken token);
}

public class Service : IService
{
    private readonly ILogger<Service> _logger;
    private readonly IMemoryCache _cache;
    private readonly Translator _translator;

    public Service(
        ILogger<Service> logger,
        IOptions<Configuration> options,
        HttpClient client,
        IMemoryCache cache
    )
    {
        _logger = logger;
        _cache = cache;
        var config = options.Value;
        _translator = new Translator(
            config.DeepLApiKey,
            new TranslatorOptions
            {
                sendPlatformInfo = false,
                ServerUrl = config.BaseUri.ToString(),
                ClientFactory = () =>
                    new HttpClientAndDisposeFlag { HttpClient = client, DisposeClient = false },
            }
        );
    }

    public async Task<string> TranslateAsync(string text, CancellationToken token)
    {
        var key = GetCacheKey(text);
        if (_cache.TryGetValue(key, out string? translation))
        {
            Debug.Assert(translation != null);
            return translation;
        }

        translation = await RequestTranslationAsync(text, token);

        _cache.Set(key, translation);

        return translation;
    }

    private async Task<string> RequestTranslationAsync(string text, CancellationToken token)
    {
        _logger.LogInformation("Translating '{Source}' into English", text);

        var result = await _translator.TranslateTextAsync(
            text,
            LanguageCode.Norwegian,
            LanguageCode.EnglishBritish,
            cancellationToken: token
        );

        return result.Text;
    }

    private static string GetCacheKey(string text)
    {
        return $"no:translation:deep-l:{text}";
    }
}
