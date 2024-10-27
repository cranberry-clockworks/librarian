using System.ComponentModel.DataAnnotations;

namespace Librarian.No.Pronunciations;

public class Configuration
{
    public string Voice { get; set; } = "nb-NO-Wavenet-C";

    [Required(AllowEmptyStrings = false, ErrorMessage = "Google credentials are required")]
    public string GoogleServiceAccountJsonCredentialsFilePath { get; set; } = string.Empty;
}
