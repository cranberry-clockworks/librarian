using Librarian.No.Dictionaries.Client;

namespace Librarian.No.Dictionaries;

public static class DependencyInjectionExtension
{
    public static void AddNorwegianDictionary(this WebApplicationBuilder builder)
    {
        builder.AddOrdbokClient();
        builder.Services.AddTransient<IService, Service>();
    }
}
