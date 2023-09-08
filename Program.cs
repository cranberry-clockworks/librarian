using AngleSharp.Dom;
using Librarian.Api.No;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("naob",
    client => client.BaseAddress = new Uri("https://naob.no/ordbok/"));

builder.Services.AddTransient<NorwegianDefinitionProvider>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/definition/{word}",
    ([FromRouteAttribute] string word, NorwegianDefinitionProvider pr) =>
        Task.FromResult($"{word}{word}"));

app.Run();