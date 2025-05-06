namespace CustomHttpServer.RequestHandlers;

public class RootHandler : IRequestHandler
{
    public HttpResponse HandleRequest(HttpRequest request)
    {
        Console.WriteLine("Handling root request");
        var headers = new Dictionary<string, string>();
        
        if (request.Headers.ContainsKey("Connection"))
        {
            headers.Add("Connection", "close");
        }

        return new HttpResponse
        {
            StatusCode = 200,
            StatusMessage = "OK",
            Headers = headers
        };
    }
}