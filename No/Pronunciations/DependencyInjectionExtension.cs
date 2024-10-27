namespace Librarian.No.Pronunciations;

public static class DependencyInjectionExtension
{
    public static void AddNorwegianPronunciation(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddOptions<Configuration>()
            .Bind(builder.Configuration.GetRequiredSection("Norwegian:Pronunciations"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddTransient<IService, Service>();
    }
}
