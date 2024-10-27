namespace Librarian;

public interface IRazorRenderer
{
    Task<string> RenderViewToStringAsync(
        string viewName,
        IDictionary<string, object?> viewBagValues
    );
}
