namespace CustomHttpServer.RequestHandlers;

public interface IRequestHandler
{
    /// <summary>
    /// Handles an HTTP request and returns an HTTP response.
    /// </summary>
    /// <param name="request">The HTTP request to handle.</param>
    /// <returns>An HTTP response.</returns>
    HttpResponse HandleRequest(HttpRequest request);
}