using System.Net;
using System.Net.Http.Headers;

namespace Librarian;

public class ApiException(
    string message,
    HttpStatusCode statusCode,
    string response,
    HttpHeaders headers
)
    : Exception(
        $"{message}\n\nStatus: {statusCode}\nResponse:\n{response[..(response.Length >= 512 ? 512 : response.Length)]}"
    )
{
    public HttpStatusCode StatusCode { get; private set; } = statusCode;

    public string Response { get; private set; } = response;

    public HttpHeaders Headers { get; } = headers;
}
