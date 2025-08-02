using System.Security.Cryptography.X509Certificates;
using DeepL;
using Microsoft.Extensions.Options;

namespace Librarian.Api.No;

public class TranslationServiceConfiguration
{
    public const string SectionName = "Norwegian:Translation";
    public string DeepLApiKey { get; set; } = string.Empty;
}

public class TranslationService
{
    private readonly ILogger<TranslationService> _logger;
    private readonly Translator _translator;

    public TranslationService(
        ILogger<TranslationService> logger,
        HttpClient client,
        IOptions<TranslationServiceConfiguration> options
    )
    {
        if (string.IsNullOrEmpty(options.Value.DeepLApiKey))
        {
            throw new ArgumentException("The API key for DeepL is not set", nameof(options));
        }

        _logger = logger;
        _translator = new Translator(
            options.Value.DeepLApiKey,
            new TranslatorOptions
            {
                sendPlatformInfo = false,
                ServerUrl = "https://api-free.deepl.com",
                ClientFactory = () =>
                    new HttpClientAndDisposeFlag { HttpClient = client, DisposeClient = false }
            }
        );
    }

    public async Task<string> TranslateAsync(string text, CancellationToken token)
    {
        _logger.LogInformation("Requesting the translation \"{Source}\" to English", text);

        var result = await _translator.TranslateTextAsync(
            text,
            LanguageCode.Norwegian,
            LanguageCode.EnglishBritish,
            cancellationToken: token
        );

        return result.Text;
    }
}
