using Microsoft.Extensions.Options;

namespace Librarian.Cards.Anki;

public static class DependencyInjectionExtension
{
    public static void AddAnki(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddOptions<Configuration>()
            .Bind(builder.Configuration.GetSection("Anki"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddHttpClient<IAnkiConnect, AnkiConnect>(
            (services, client) =>
            {
                client.BaseAddress = services
                    .GetRequiredService<IOptions<Configuration>>()
                    .Value.Uri;
            }
        );

        builder.Services.AddTransient<IAnkiService, AnkiService>();
        builder.Services.AddTransient<ITemplateRenderer, TemplateRenderer>();
    }
}
