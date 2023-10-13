using Librarian.Api.Clients;
using Librarian.Api.Models;
using Librarian.Api.No;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<NorwegianTranslationServiceOption>(
    builder.Configuration.GetSection(NorwegianTranslationServiceOption.SectionName)
);

builder.Services.AddHttpClient();

builder.Services.AddTransient<OrdbokClient>();
builder.Services.AddTransient<NorwegianDefinitionProvider>();
builder.Services.AddTransient<NorwegianTranslationService>();

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
        ([FromRouteAttribute] string phrase, NorwegianTranslationService service) =>
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
app.Run();
