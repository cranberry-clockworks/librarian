using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Librarian.Api.No.Definitions;

public static class Endpoint
{
    public static void AddDefinitionServices(this IServiceCollection services)
    {
        services.AddHttpClient();

        services.AddTransient<OrdbokClient.OrdbokClient>();
        services.AddTransient<DefinitionService>();
    }

    public static void MapDefinitionEndpoints(this WebApplication app)
    {
        app.MapGet("/api/definition/{word}", Handle)
            .Produces<ICollection<Definition>>()
            .WithName("Define")
            .WithOpenApi(
                operation =>
                    new OpenApiOperation(operation)
                    {
                        Summary = "Defines the word",
                        Tags = new List<OpenApiTag> { new() { Name = "Definitions" } }
                    }
            );
    }

    private static async Task<IResult> Handle(
        [FromRoute] string word,
        [FromQuery] string? partOfSpeech,
        [FromQuery] int? count,
        [FromServices] DefinitionService service
    )
    {
        var result = await service.GetDefinitionsAsync(
            word,
            partOfSpeech,
            count ?? 10,
            CancellationToken.None
        );
        return Results.Ok(result);
    }
}
