using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Librarian.Api.Anki;

public static class Endpoint
{
    public static void AddAnkiServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddTransient<AnkiConnect>();
        services.AddTransient<AnkiService>();
    }

    public static void MapAnkiEndpoints(this WebApplication app)
    {
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
    }
}
