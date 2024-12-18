using System.Text;
using System.Text.RegularExpressions;
using PronunciationService = Librarian.No.Pronunciations.IService;
using Template = Librarian.Cards.Card;
using TranslationService = Librarian.No.Translations.IService;

namespace Librarian.Cards.Anki;

public class TemplateRenderer(
    TranslationService translationService,
    PronunciationService pronunciationService,
    IRazorRenderer razorRenderer
) : ITemplateRenderer
{
    public async Task<List<Card>> RenderAsync(
        IEnumerable<Template> templates,
        CancellationToken token
    )
    {
        var converted = new List<Card>();
        foreach (var template in templates)
        {
            var audioName = GetAudioName(template.Phrase);
            var audioTask = pronunciationService.PronounceAsync(template.Phrase, token);

            var translationTask = string.IsNullOrEmpty(template.Translation)
                ? translationService.TranslateAsync(template.Phrase, token)
                : Task.FromResult(template.Translation);

            var frontTask = RenderFront(template, audioName, token);
            var backTask = RenderBack(template, translationTask, token);

            await Task.WhenAll(audioTask, frontTask, backTask);

            var card = new Card()
            {
                Front = await frontTask,
                Back = await backTask,
                Media = new Dictionary<string, string>
                {
                    { audioName, Convert.ToBase64String((await audioTask).Content) },
                },
            };

            converted.Add(card);
        }

        return converted;
    }

    private string GetAudioName(string phrase)
    {
        var bytes = Encoding.UTF8.GetBytes(phrase);
        var base64 = Convert.ToBase64String(bytes);
        return InvalidFileNameChars.Replace(base64, "");
    }

    private Task<string> RenderFront(Template card, string audioName, CancellationToken token)
    {
        return razorRenderer.RenderViewToStringAsync(
            "Anki/Front",
            new Dictionary<string, object?>
            {
                { "Phrase", card.Phrase },
                { "PartOfSpeech", card.PartOfSpeech },
                { "Tags", card.Tags },
                { "AudioName", audioName },
                { "ExampleNorwegian", card.NorwegianUsageExample }
            }
        );
    }

    private async Task<string> RenderBack(
        Template card,
        Task<string> translationTask,
        CancellationToken token
    )
    {
        return await razorRenderer.RenderViewToStringAsync(
            "Anki/Back",
            new Dictionary<string, object?>
            {
                { "PartOfSpeech", card.PartOfSpeech },
                { "Tags", card.Tags },
                { "Translation", await translationTask },
                { "ExampleEnglish", card.EnglishUsageExample }
            }
        );
    }

    private static readonly Regex InvalidFileNameChars =
        new(
            "[^a-zA-Z0-9]",
            RegexOptions.Compiled | RegexOptions.NonBacktracking,
            TimeSpan.FromSeconds(1)
        );
}
