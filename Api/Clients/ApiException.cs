using System.Net;
using System.Net.Http.Headers;

namespace Librarian.Api.Clients;

public class ApiException : Exception
{
    public ApiException(string message, HttpStatusCode statusCode, string response, HttpHeaders headers)
        : base(
            $"{message}\n\nStatus: {statusCode}\nResponse:\n{response[..(response.Length >= 512 ? 512 : response.Length)]}")
    {
        StatusCode = statusCode;
        Response = response;
        Headers = headers;
    }

    public HttpStatusCode StatusCode { get; private set; }

    public string Response { get; private set; }

    public HttpHeaders Headers { get; }
}