using Microsoft.AspNetCore.Mvc;

namespace Librarian.Cards;


/// <summary>
/// Card model used for the form to add.
/// </summary>
public record Card
{
    /// <summary>
    /// A word or set of words.
    /// </summary>
    [FromForm(Name = "phrase")]
    public required string Phrase { get; init; }

    /// <summary>
    /// Additional tags related to the part of speech or phrase.
    /// </summary>
    [FromForm(Name = "tags")]
    public string[] Tags { get; init; } = Array.Empty<string>();

    /// <summary>
    /// Part of speech of the <see cref="Phrase"/>.
    /// </summary>
    [FromForm(Name = "part-of-speech")]
    public required string PartOfSpeech { get; init; }

    /// <summary>
    /// The translation of the <see cref="Phrase"/>.
    /// </summary>
    [FromForm(Name = "translation")]
    public string? Translation { get; init; }
    
    public string? NorwegianUsageExample { get; init; }
    public string? EnglishUsageExample { get; init; }

    /// <summary>
    /// The ID that represent the card uniquely.
    /// </summary>
    public int Id => HashCode.Combine(
        Phrase,
        PartOfSpeech,
        string.Join("",Tags.OrderBy(static x => x)));
}
