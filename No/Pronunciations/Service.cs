using System.Diagnostics;
using Google.Api.Gax.Grpc.Rest;
using Google.Cloud.TextToSpeech.V1;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Librarian.No.Pronunciations;

public class Service : IService
{
    private readonly ILogger<Service> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly VoiceSelectionParams _voiceSelectionParams;
    private readonly TextToSpeechClient _client;

    private static readonly AudioConfig AudioConfig = new() { AudioEncoding = AudioEncoding.Mp3 };

    private const string ContentType = "audio/mpeg";

    public Service(
        ILogger<Service> logger,
        IOptions<Configuration> options,
        IMemoryCache memoryCache
    )
    {
        _logger = logger;
        _memoryCache = memoryCache;

        _voiceSelectionParams = new VoiceSelectionParams()
        {
            LanguageCode = "no-bok",
            Name = options.Value.Voice,
        };

        var relativePath = Path.GetRelativePath(
            Environment.CurrentDirectory,
            Path.GetFullPath(options.Value.GoogleServiceAccountJsonCredentialsFilePath)
        );

        _client = new TextToSpeechClientBuilder()
        {
            CredentialsPath = relativePath,
            GrpcAdapter = RestGrpcAdapter.Default,
        }.Build();
    }

    private async Task<Audio> RequestPronunciationAsync(string phrase, CancellationToken token)
    {
        _logger.LogInformation("Converting '{Phrase}' to speech", phrase);

        var input = new SynthesisInput() { Text = phrase };

        var response = await _client.SynthesizeSpeechAsync(
            input,
            _voiceSelectionParams,
            AudioConfig,
            token
        );

        return new Audio(ContentType, response.AudioContent.ToByteArray());
    }

    public async Task<Audio> PronounceAsync(string phrase, CancellationToken token)
    {
        var key = GetCacheKey(phrase);

        if (_memoryCache.TryGetValue(key, out Audio? audio))
        {
            Debug.Assert(audio != null);
            return audio;
        }

        audio = await RequestPronunciationAsync(phrase, token);
        _memoryCache.Set(key, audio, CacheExpiration);

        return audio;
    }

    private static string GetCacheKey(string phrase)
    {
        return $"no:pronunciation:google:{phrase}";
    }

    private static readonly DateTimeOffset CacheExpiration = DateTimeOffset.UtcNow.AddDays(2);
}
