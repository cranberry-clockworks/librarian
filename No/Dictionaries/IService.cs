namespace Librarian.No.Dictionaries;

public interface IService
{
    Task<IReadOnlyCollection<Definition>> GetDefinitionsAsync(
        string phrase,
        PartOfSpeech partOfSpeech,
        CancellationToken token
    );
}
