namespace CustomHttpServer.RequestHandlers;

public class FileHandler(string directory) : IRequestHandler
{
    public HttpResponse HandleRequest(HttpRequest request)
    {
        var fileName = request.Path.Contains("/files/")?
        request.Path.Substring(request.Path.IndexOf("/files/") + 7) :
            string.Empty;

        var filePath = Path.Combine(directory + fileName);

        switch (request.Method)
        {
            case "GET":
                return HandleGetRequest(filePath);
            case "POST":
                return HandlePostRequest(filePath, request.Body);
            default:
                return new HttpResponse(404);
        }
    }

    private static HttpResponse HandleGetRequest(string filePath)
    {
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

    private static HttpResponse HandlePostRequest(string filePath, string content)
    {
        File.WriteAllText(filePath, content);
        return new HttpResponse(201);
    }
}