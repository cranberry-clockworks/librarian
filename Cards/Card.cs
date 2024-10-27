using Microsoft.AspNetCore.Mvc;

namespace Librarian.Cards;

public class Card
{
    [FromForm(Name = "phrase")]
    public required string Phrase { get; init; }

    [FromForm(Name = "tags")]
    public string? Tags { get; init; }

    [FromForm(Name = "part-of-speech")]
    public required string PartOfSpeech { get; init; }

    [FromForm(Name = "translation")]
    public string? Translation { get; init; }

    public int Id => HashCode.Combine(Phrase, PartOfSpeech, Tags, Translation);
}
