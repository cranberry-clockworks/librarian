namespace Librarian.No.Pronunciations;

public interface IService
{
    Task<Audio> PronounceAsync(string phrase, CancellationToken token);
}
