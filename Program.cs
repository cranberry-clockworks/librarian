using System.ComponentModel.DataAnnotations;
using Librarian;
using Librarian.Api.Anki;
using Librarian.Api.Models;
using Librarian.Api.Models.Definitions;
using Librarian.Api.No;
using Librarian.Api.No.Definitions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using WordClass = Librarian.Api.Models.WordClass;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<TranslationServiceConfiguration>(
    builder.Configuration.GetSection(TranslationServiceConfiguration.SectionName)
);

builder.Services.Configure<PronunciationServiceConfiguration>(
    builder.Configuration.GetSection(PronunciationServiceConfiguration.Section)
);

builder.Services.AddHttpClient();

builder.Services.AddDefinitionService();
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

app.MapDefinitionEndpoint();

app.MapGet(
        "/api/translation/{phrase}",
        ([FromRoute] string phrase, [FromServices] TranslationService service) =>
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
        async ([FromRoute] string phrase, [FromServices] PronunciationService service) =>
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
        ([FromBody] ExportRequest request, [FromServices] AnkiService service) =>
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

app.MapGet(
        "/api/anki/decks",
        ([FromServices] AnkiConnect client) => client.GetDecks(CancellationToken.None)
    )
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
