using Librarian.Api.Clients;
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
builder.Services.AddTransient<NorwegianDefinitionProvider>();
builder.Services.AddTransient<TranslationService>();
builder.Services.AddTransient<PronunciationService>();

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
        async ([FromRouteAttribute] string word, NorwegianDefinitionProvider pr) =>
            await pr.GetDefinitionsAsync(word, 3)
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
        ([FromRouteAttribute] string phrase, TranslationService service) =>
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
        async ([FromRouteAttribute] string phrase, PronunciationService service) =>
        {
            var bytes = await service.PronounceAsync(phrase, CancellationToken.None);
            return $"data:audio/mpeg;base64,{Convert.ToBase64String(bytes)}";
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

app.Run();
