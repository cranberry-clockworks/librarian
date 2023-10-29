using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace Librarian.Api.Anki;

public class AnkiConnectConfiguration
{
    public const string Section = "AnkiConnect";
    public Uri Uri { get; set; } = new Uri("http://localhost:8765");
}

public class AnkiConnect
{
    private readonly ILogger<AnkiConnect> _logger;
    private readonly HttpClient _client;

    public AnkiConnect(
        ILogger<AnkiConnect> logger,
        HttpClient client,
        IOptions<AnkiConnectConfiguration> configuration
    )
    {
        _logger = logger;
        _client = client;
        _client.BaseAddress = configuration.Value.Uri;
    }

    public Task StoreMediaFileAsync(string name, string content, CancellationToken token)
    {
        _logger.LogInformation("Storing media file: {Name}", name);

        var data = new Dictionary<string, object>()
        {
            { "action", "storeMediaFile" },
            { "version", 6 },
            {
                "params",
                new Dictionary<string, string> { { "filename", name }, { "data", content } }
            }
        };

        return ExecuteAsync<AnkiConnectResponse>("store media file", data, token);
    }

    public Task AddNoteAsync(string deck, string front, string back, CancellationToken token)
    {
        _logger.LogInformation("Adding new note to the deck: {Deck}", deck);

        var command = new Dictionary<string, object>()
        {
            { "action", "addNote" },
            { "version", 6 },
            {
                "params",
                new Dictionary<string, object>
                {
                    {
                        "note",
                        new Dictionary<string, object>
                        {
                            { "deckName", deck },
                            { "modelName", "Basic (and reversed card)" },
                            {
                                "fields",
                                new Dictionary<string, string>
                                {
                                    { "Front", front },
                                    { "Back", back },
                                }
                            },
                        }
                    }
                }
            }
        };

        return ExecuteAsync<AnkiConnectResponse>("add note", command, token);
    }

    public Task SyncAsync(CancellationToken token)
    {
        _logger.LogInformation("Requesting synchronization");
        var command = new Dictionary<string, object> { { "action", "sync" }, { "version", 6 }, };
        return ExecuteAsync<AnkiConnectResponse>("sync", command, token);
    }

    public async Task<List<string>> GetDecks(CancellationToken token)
    {
        _logger.LogInformation("Getting list decks");
        var command = new Dictionary<string, object>
        {
            { "action", "deckNames" },
            { "version", 6 }
        };

        var response = await ExecuteAsync<Decks>("deckNames", command, token);
        return response.Response;
    }

    private async Task<T> ExecuteAsync<T>(
        string action,
        Dictionary<string, object> command,
        CancellationToken token
    )
        where T : AnkiConnectResponse
    {
        var json = JsonSerializer.Serialize(command, _serializerOptions);

        using var request = new HttpRequestMessage(HttpMethod.Post, "/");
        request.Content = new StringContent(json);
        request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json)
        );

        var response = await _client.SendAsync(request, token);

        // API returns error in a special field instead of the response code
        Debug.Assert(response.StatusCode == HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync(token);
        try
        {
            var result = JsonSerializer.Deserialize<T>(responseContent);

            if (result?.Error != null)
            {
                throw new ApiException(
                    $"Failed to perform action: {action}",
                    response.StatusCode,
                    result.Error ?? string.Empty,
                    response.Headers
                );
            }

            return result!;
        }
        catch
        {
            throw new ApiException(
                $"Failed to perform action: {action}",
                response.StatusCode,
                $"Failed to deserialize response: {responseContent}",
                response.Headers
            );
        }
    }

    private readonly JsonSerializerOptions _serializerOptions = new() { WriteIndented = false };

    private class AnkiConnectResponse
    {
        [JsonPropertyName("error")]
        public string? Error { get; init; }
    }

    private class Decks : AnkiConnectResponse
    {
        [JsonPropertyName("result")]
        public List<string> Response { get; init; } = new();
    }
}
