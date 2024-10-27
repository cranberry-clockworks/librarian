using System.ComponentModel.DataAnnotations;

namespace Librarian.No.Translations;

public class Configuration
{
    public Uri BaseUri { get; set; } = new("https://api-free.deepl.com");

    [Required(AllowEmptyStrings = false, ErrorMessage = "DeepL credentials are required")]
    public string DeepLApiKey { get; set; } = string.Empty;
}
