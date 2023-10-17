using Google.Api.Gax.Grpc.Rest;
using Google.Cloud.TextToSpeech.V1;
using Microsoft.Extensions.Options;

namespace Librarian.Api.No;

public class PronunciationServiceConfiguration
{
    public const string Section = "Norwegian:Pronunciation";

    public string Voice { get; set; } = "nb-NO-Wavenet-C";
    public string GoogleServiceAccountJsonCredentialsFilePath { get; set; } = string.Empty;
}

public class PronunciationService
{
    private readonly ILogger<PronunciationService> _logger;
    private readonly VoiceSelectionParams _voiceSelectionParams;
    private readonly TextToSpeechClient _client;

    private static readonly AudioConfig AudioConfig = new() { AudioEncoding = AudioEncoding.Mp3 };

    public PronunciationService(
        ILogger<PronunciationService> logger,
        IOptions<PronunciationServiceConfiguration> options
    )
    {
        _logger = logger;

        _voiceSelectionParams = new VoiceSelectionParams()
        {
            LanguageCode = "no-bok",
            Name = options.Value.Voice
        };

        var relativePath = Path.GetRelativePath(
            Environment.CurrentDirectory,
            Path.GetFullPath(options.Value.GoogleServiceAccountJsonCredentialsFilePath)
        );

        _client = new TextToSpeechClientBuilder()
        {
            CredentialsPath = relativePath,
            GrpcAdapter = RestGrpcAdapter.Default
        }.Build();
    }

    public async Task<byte[]> PronounceAsync(string phrase, CancellationToken token)
    {
        _logger.LogInformation("Converting {Phrase} to speech", phrase);

        var input = new SynthesisInput() { Text = phrase };

        var response = await _client.SynthesizeSpeechAsync(
            input,
            _voiceSelectionParams,
            AudioConfig,
            token
        );

        return response.AudioContent.ToByteArray();
    }
}
