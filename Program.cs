using Librarian;
using Librarian.Api.Anki;
using Librarian.Api.Models;
using Librarian.Api.No;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<TranslationServiceConfiguration>(
    builder.Configuration.GetSection(TranslationServiceConfiguration.SectionName)
);

builder.Services.Configure<PronunciationServiceConfiguration>(
    builder.Configuration.GetSection(PronunciationServiceConfiguration.Section)
);

builder.Services.AddHttpClient();

builder.Services.AddTransient<OrdbokClient>();
builder.Services.AddTransient<DefinitionService>();
builder.Services.AddTransient<TranslationService>();
builder.Services.AddTransient<PronunciationService>();

builder.Services.AddTransient<AnkiConnect>();
builder.Services.AddTransient<AnkiService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "Server-side API", Version = "v1" });
    o.SupportNonNullableReferenceTypes();
    o.UseOneOfForPolymorphism();
    o.EnableAnnotations(
        enableAnnotationsForPolymorphism: true,
        enableAnnotationsForInheritance: true
    );
});

var app = builder.Build();

app.UseSwagger();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet(
        "/api/definition/{word}",
        ([FromRoute] string word, DefinitionService pr) => pr.GetDefinitionsAsync(word, 3)
    )
    // .Produces<NounDefinition>(StatusCodes.Status200OK)
    .WithName("Define")
    .WithOpenApi(
        operation =>
            new OpenApiOperation(operation)
            {
                Summary = "Defines the word",
                Tags = new List<OpenApiTag> { new() { Name = "Definitions" } }
            }
    );

app.MapGet(
        "/api/translation/{phrase}",
        ([FromRoute] string phrase, TranslationService service) =>
            service.TranslateAsync(phrase, CancellationToken.None)
    )
    .WithName("Translate")
    .WithOpenApi(
        operation =>
            new OpenApiOperation(operation)
            {
                Summary = "Translates the phrase",
                Tags = new List<OpenApiTag> { new() { Name = "Translations" } }
            }
    );

app.MapGet(
        "/api/pronunciation/{phrase}",
        async ([FromRoute] string phrase, PronunciationService service) =>
        {
            var bytes = await service.PronounceAsync(phrase, CancellationToken.None);
            return $"{Media.FormatPrefix}{Convert.ToBase64String(bytes)}";
        }
    )
    .WithName("Pronounce")
    .WithOpenApi(
        operation =>
            new OpenApiOperation(operation)
            {
                Summary = "Pronounces the phrase",
                Tags = new List<OpenApiTag> { new() { Name = "Pronunciations" } }
            }
    );

app.MapPost(
        "/api/anki/export",
        ([FromBody] ExportRequest request, AnkiService service) =>
            service.AddCards(request.Deck, request.Cards, CancellationToken.None)
    )
    .WithName("Export")
    .WithOpenApi(
        operation =>
            new OpenApiOperation(operation)
            {
                Summary = "Export cards",
                Tags = new List<OpenApiTag> { new() { Name = "Anki" } }
            }
    );

app.MapGet("/api/anki/decks", (AnkiConnect client) => client.GetDecks(CancellationToken.None))
    .WithName("GetDecks")
    .WithOpenApi(
        operation =>
            new OpenApiOperation(operation)
            {
                Summary = "Get available decks",
                Tags = new List<OpenApiTag> { new() { Name = "Anki" } }
            }
    );

app.Run();
