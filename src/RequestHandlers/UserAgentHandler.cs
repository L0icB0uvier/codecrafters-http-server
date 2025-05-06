namespace CustomHttpServer.RequestHandlers;

public class UserAgentHandler : IRequestHandler
{
    public HttpResponse HandleRequest(HttpRequest request)
    {
        Console.WriteLine("Handling user-agent request");

        var headers = new Dictionary<string, string>(){ ["Content-Type"] = "text/plain" };
        
        if (request.Headers.ContainsKey("Connection"))
        {
            headers.Add("Connection", "close");
        } 
        
        string userAgent = String.Empty;
        if (request.Headers.TryGetValue("User-Agent", out string? value))
        {
            userAgent = value;
        }
        
        return new HttpResponse
        {
            StatusCode = 200,
            StatusMessage = "OK",
            Headers = headers,
            Body = userAgent
        };
    }
}