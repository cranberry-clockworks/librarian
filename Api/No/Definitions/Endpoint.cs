using Librarian.Api.Models.Definitions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Librarian.Api.No.Definitions;

public static class Endpoint
{
    public static void AddDefinitionService(this IServiceCollection services)
    {
        // services.AddHttpClient();

        services.AddTransient<OrdbokClient>();
        services.AddTransient<DefinitionService>();
    }

    public static void MapDefinitionEndpoint(this WebApplication app)
    {
        app.MapGet("/api/definition/{word}", Handle)
            .Produces<ICollection<Definition>>()
            .ProducesValidationProblem()
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
        [FromServices] DefinitionService pr
    )
    {
        var result = await pr.GetDefinitionsAsync(
            word,
            partOfSpeech,
            count ?? 3,
            CancellationToken.None
        );
        return Results.Ok(result);
    }
}
