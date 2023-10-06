using Librarian.Api.Clients;
using Librarian.Api.Models;
using Librarian.Api.No;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddTransient<OrdbokClient>();
builder.Services.AddTransient<NorwegianDefinitionProvider>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
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
    .WithOpenApi();

app.Run();
