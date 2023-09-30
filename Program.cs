using Librarian.Api.Clients;
using Librarian.Api.No;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddTransient<OrdbokClient>();
builder.Services.AddTransient<NorwegianDefinitionProvider>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/definition/{word}",
    async ([FromRouteAttribute] string word, NorwegianDefinitionProvider pr) =>
        await pr.GetDefinitionsAsync(word));

app.Run();