namespace CustomHttpServer.RequestHandlers;

public class FileHandler(string directory) : IRequestHandler
{
    public HttpResponse HandleRequest(HttpRequest request)
    {
        
        var fileName = request.Path.Contains("/file/")?
        request.Path.Substring(request.Path.IndexOf("/files/") + 7) :
            string.Empty;

        var filePath = Path.Combine(directory + fileName);

        if (!File.Exists(filePath))
            return new HttpResponse(404);
        
        var contents = File.ReadAllText(filePath);
        if (contents.Length == 0)
        {
            return new HttpResponse(404);
        }
        
        return new HttpResponse
        {
            StatusCode = 200,
            StatusMessage = "OK",
            Headers = {["Content-Type"] = "application/octet-stream"},
            Body = contents
        };
    }
}