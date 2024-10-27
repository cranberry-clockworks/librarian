using Microsoft.Extensions.Options;

namespace Librarian.No.Dictionaries.Client;

public static class DependencyInjectionExtension
{
    public static void AddOrdbokClient(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddOptions<Configuration>()
            .Bind(builder.Configuration.GetRequiredSection("Norwegian:Ordbok"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddHttpClient<IOrdbokClient, OrdbokClient>(
            static (services, client) =>
            {
                var baseUrl = services.GetRequiredService<IOptions<Configuration>>().Value.BaseUrl;
                client.BaseAddress = baseUrl;
            }
        );
    }
}
