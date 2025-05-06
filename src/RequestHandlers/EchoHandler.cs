namespace CustomHttpServer.RequestHandlers;

public class EchoHandler : IRequestHandler
{
    public HttpResponse HandleRequest(HttpRequest request)
    {
        Console.WriteLine("Handling echo request");
        string content = request.Path.Contains("/echo/")
            ? request.Path.Substring(request.Path.IndexOf("/echo/") + 6)
            : string.Empty;

        var useCompression = false;
        if (request.Headers.TryGetValue("Accept-Encoding", out string? encodings))
        {
            useCompression = encodings.Split(',')
                .Select(e => e.Trim().ToLowerInvariant())
                .Contains("gzip");
            Console.WriteLine($"Use compression: {useCompression}");
        }
        
        var headers = new Dictionary<string, string> {["Content-Type"] = "text/plain" };
        
        if (request.Headers.ContainsKey("Connection"))
        {
            headers.Add("Connection", "close");
        }

        return new HttpResponse
        {
            StatusCode = 200,
            StatusMessage = "OK",
            Headers = headers, 
            Body = content,
            UseCompression = useCompression
        };
    }
}