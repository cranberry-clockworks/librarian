using System.Text;

namespace Librarian;

internal static class UriExtension
{
    /// <summary>
    ///     Builds URI string with query parameters.
    /// </summary>
    /// <param name="path">
    ///     Base URI path.
    /// </param>
    /// <param name="pairs">
    ///     Keys and values for query parameters. The values should be escaped.
    /// </param>
    /// <returns>
    ///     URI with query parameters. Example: <c>path?key1=value1&amp;key2=value2</c>
    /// </returns>
    public static string BuildUriWithQueryParameters(
        string path,
        IEnumerable<(string, string?)> pairs
    )
    {
        var builder = new StringBuilder(path);

        var first = true;

        foreach (var (key, value) in pairs)
        {
            if (value == null)
                continue;

            builder.Append(first ? '?' : '&');
            builder.Append(key);
            builder.Append('=');
            builder.Append(value);

            first = false;
        }

        return builder.ToString();
    }
}
