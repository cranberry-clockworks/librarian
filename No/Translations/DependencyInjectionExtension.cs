namespace Librarian.No.Translations;

public static class DependencyInjectionExtension
{
    public static void AddNorwegianTranslation(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddOptions<Configuration>()
            .Bind(builder.Configuration.GetRequiredSection("Norwegian:Translations"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddHttpClient<IService, Service>();
    }
}
