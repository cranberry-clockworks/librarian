namespace Librarian.No.Dictionaries;

public interface IService
{
    Task<IReadOnlyCollection<int>> GetArticlesAsync(string phrase, PartOfSpeech partOfSpeech, CancellationToken token);

    public Task<IReadOnlyCollection<Definition>> GetDefinitionsAsync(
        IEnumerable<int> articleIds,
        CancellationToken token
    );
}
