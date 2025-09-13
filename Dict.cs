#:package CommandLineParser@2.9.1
#:package Scriban@6.3.0

using CommandLine;
using System.Text.Json;
using System.Text.Json.Serialization;
using Scriban;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Linq;

var exitCode = await Parser
    .Default.ParseArguments<PackVerb, ImportVerb>(args)
    .MapResult(
        async (PackVerb pack) => await RunPackAndReturnExitCode(pack),
        async (ImportVerb import) => await RunImportAndReturnExitCode(import),
        errs => Task.FromResult(1)
    );

Environment.Exit(exitCode);

[UnconditionalSuppressMessage(
    "Trimming",
    "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
    Justification = "Using JsonSerializerContext for AOT compatibility"
)]
async Task<int> RunPackAndReturnExitCode(PackVerb pack)
{
    try
    {
        var input = await Console.In.ReadToEndAsync();
        var words = JsonSerializer.Deserialize(
            input,
            AppJsonContext.Default.EntryArray
        );

        if (words == null)
        {
            await Console.Error.WriteLineAsync("Invalid JSON input");
            return 1;
        }

        var frontTemplate = Template.Parse(
            @"
<div>
    <h1>{{native}}</h1>
    <p><em>{{phonetics}}</em></p>
    <small>{{tag}}</small>
    <br/>
    <p>{{usage}}</p>
</div>"
        );

        var backTemplate = Template.Parse(
            @"
<div>
    <h1>{{translation}}</h1>
    <small>{{tag}}</small>
    <br/>
    <p>{{usage}}</p>
</div>"
        );

        var ankiCards = new List<AnkiNote>();

        foreach (var word in words)
        {
            var frontHtml = await frontTemplate.RenderAsync(
                new
                {
                    native = word.Native,
                    phonetics = word.Phonetics,
                    tag = word.Tag,
                    usage = word.UsageNative
                }
            );

            var backHtml = await backTemplate.RenderAsync(
                new
                {
                    translation = word.Translation,
                    tag = word.Tag,
                    usage = word.UsageTranslated
                }
            );

            ankiCards.Add(
                new AnkiNote
                {
                    Front = frontHtml.Trim(),
                    Back = backHtml.Trim()
                }
            );
        }

        var json = JsonSerializer.Serialize(
            ankiCards.ToArray(),
            AppJsonContext.Default.AnkiNoteArray
        );

        Console.WriteLine(json);
        return 0;
    }
    catch (Exception ex)
    {
        await Console.Error.WriteLineAsync($"Error: {ex.Message}");
        return 1;
    }
}

async Task<int> RunImportAndReturnExitCode(ImportVerb import)
{
    try
    {
        if (import.List)
        {
            return await ListDecksAndReturnExitCode(import.Url);
        }
        else
        {
            return await ImportCardsAndReturnExitCode(import);
        }
    }
    catch (Exception ex)
    {
        await Console.Error.WriteLineAsync($"Error: {ex.Message}");
        return 1;
    }
}

async Task<int> ListDecksAndReturnExitCode(string url)
{
    try
    {
        using var client = new HttpClient();

        var content = new StringContent(
            "{\"action\": \"deckNames\", \"version\": 5}",
            System.Text.Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var deckResponse = JsonSerializer.Deserialize(
            responseJson,
            AppJsonContext.Default.DeckListResponse
        );

        if (deckResponse?.Error != null)
        {
            await Console.Error.WriteLineAsync(
                $"AnkiConnect error: {deckResponse.Error}"
            );
            return 1;
        }

        if (deckResponse?.Result != null)
        {
            foreach (var deck in deckResponse.Result)
            {
                Console.WriteLine(deck);
            }
        }
        return 0;
    }
    catch (Exception ex)
    {
        await Console.Error.WriteLineAsync($"Error listing decks: {ex.Message}");
        return 1;
    }
}

async Task<int> ImportCardsAndReturnExitCode(ImportVerb import)
{
    try
    {
        var input = await Console.In.ReadToEndAsync();
        var cards = JsonSerializer.Deserialize(
            input,
            AppJsonContext.Default.AnkiNoteArray
        );

        if (cards == null)
        {
            await Console.Error.WriteLineAsync("Invalid JSON input");
            return 1;
        }

        using var client = new HttpClient();

        // Build concrete AddNote[] array instead of anonymous objects
        var notes = cards.Select(card => new AddNote
        {
            DeckName = import.Deck,
            ModelName = import.NoteType,
            Fields = new NoteFields { Front = card.Front, Back = card.Back },
            Tags = (import.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(t => t.Trim())
                        .Where(t => !string.IsNullOrEmpty(t))
                        .ToArray())
                    ?? Array.Empty<string>()
        }).ToArray();

        var request = new AnkiConnectRequest
        {
            Action = "addNotes",
            Version = 6,
            Params = new AddNotesParams { Notes = notes }
        };

        var json = JsonSerializer.Serialize(
            request,
            AppJsonContext.Default.AnkiConnectRequest
        );
        var content = new StringContent(
            json,
            System.Text.Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync(import.Url, content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var ankiResponse = JsonSerializer.Deserialize(
            responseJson,
            AppJsonContext.Default.AnkiConnectResponse
        );

        if (ankiResponse?.Error != null)
        {
            await Console.Error.WriteLineAsync(
                $"AnkiConnect error: {ankiResponse.Error}"
            );
            return 1;
        }

        Console.WriteLine(
            $"Successfully imported {cards.Length} cards into deck: {import.Deck}"
        );
        return 0;
    }
    catch (Exception ex)
    {
        await Console.Error.WriteLineAsync($"Error importing cards: {ex.Message}");
        return 1;
    }
}

// --- JSON source-gen annotations: include the concrete types we now use ---
[JsonSerializable(typeof(Entry[]))]
[JsonSerializable(typeof(AnkiNote[]))]
[JsonSerializable(typeof(AnkiConnectRequest))]
[JsonSerializable(typeof(AnkiConnectResponse))]
[JsonSerializable(typeof(DeckListResponse))]
[JsonSerializable(typeof(AddNote[]))]
[JsonSerializable(typeof(AddNotesParams))]
[JsonSerializable(typeof(NoteFields))]
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
)]
internal partial class AppJsonContext : JsonSerializerContext { }

// --- concrete types for Anki 'addNotes' payload ---
public class AddNotesParams
{
    public required AddNote[] Notes { get; set; }
}

public class AddNote
{
    public required string DeckName { get; set; }
    public required string ModelName { get; set; }
    public required NoteFields Fields { get; set; }
    public string[]? Tags { get; set; }
}

public class NoteFields
{
    public required string Front { get; set; }
    public required string Back { get; set; }
}

public class AnkiConnectRequest
{
    public required string Action { get; set; }
    public required int Version { get; set; }
    public AddNotesParams? Params { get; set; }
}

public class AnkiConnectResponse
{
    public object? Result { get; set; }
    public string? Error { get; set; }
}

public class DeckListResponse
{
    public string[]? Result { get; set; }
    public string? Error { get; set; }
}

public class AnkiNote
{
    public required string Front { get; set; }
    public required string Back { get; set; }
}

public class Entry
{
    public required string Native { get; set; }
    public required string Phonetics { get; set; }
    public required string Translation { get; set; }
    public required string Tag { get; set; }
    public required string UsageNative { get; set; }
    public required string UsageTranslated { get; set; }
}

[Verb("pack", HelpText = "Convert words JSON collection into the Anki deck format")]
public class PackVerb { }

[Verb("import", HelpText = "Import Anki cards into the specified deck")]
public class ImportVerb
{
    [Option('l', "list", SetName = "list", HelpText = "List available Anki decks")]
    public bool List { get; set; }

    [Option(
        'd',
        "deck",
        SetName = "import",
        HelpText = "Target deck name for importing cards"
    )]
    public string Deck { get; set; } = string.Empty;

    [Option(
        't',
        "tags",
        SetName = "import",
        HelpText = "Comma-separated tags to add to cards",
        Default = ""
    )]
    public string Tags { get; set; } = string.Empty;

    [Option('n', "note-type", SetName = "import", HelpText = "Anki note type/model to use", Default = "Basic")]
    public string NoteType { get; set; } = "Basic";

    [Option(
        'u',
        "url",
        HelpText = "AnkiConnect URL",
        Default = "http://localhost:8765"
    )]
    public string Url { get; set; } = "http://localhost:8765";
}

