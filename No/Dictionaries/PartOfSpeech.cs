namespace Librarian.No.Dictionaries;

public enum PartOfSpeech
{
    Any,
    Noun,
    Verb,
    Adjective,
}

public static class PartOfSpeechExtensions
{
    private static readonly string[] Supported = Enum.GetNames<PartOfSpeech>().ToArray();

    public static IReadOnlyList<string> SupportedPartOfSpeeches => Supported;
}
