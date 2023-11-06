using Librarian;
using Librarian.Api.Anki;
using Librarian.Api.No;
using Librarian.Api.No.Definitions;
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

builder.Services.AddDefinitionServices();
builder.Services.AddTransient<TranslationService>();
builder.Services.AddTransient<PronunciationService>();

builder.Services.AddAnkiServices();

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

app.MapDefinitionEndpoints();

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

app.MapAnkiEndpoints();

app.Run();
