using System.Text;

namespace CustomHttpServer;

public class HttpResponse
{
    public string ProtocolVersion { get; set; } = "HTTP/1.1";
    public int StatusCode { get; set; } = 200;
    public string StatusMessage { get; set; } = "OK";

    public Dictionary<string, string> Headers { get; set; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public string? Body { get; set; }

    public bool UseCompression { get; set; }

    public static Dictionary<int, string> CommonStatusMessages = new()
    {
        { 200, "OK" },
        { 201, "Created" },
        { 204, "No Content" },
        { 400, "Bad Request" },
        { 401, "Unauthorized" },
        { 403, "Forbidden" },
        { 404, "Not Found" },
        { 500, "Internal Server Error" },
        { 501, "Not Implemented" },
        { 503, "Service Unavailable" }
    };

    public HttpResponse(int statusCode = 200)
    {
        StatusCode = statusCode;
        StatusMessage = CommonStatusMessages.GetValueOrDefault(statusCode, "Unknown");
    }

    public byte[] ToByteArray()
    {
        byte[] bodyBytes = null;

        if (!string.IsNullOrEmpty(Body))
        {
            if (UseCompression)
            {
                // Compress the body
                bodyBytes = CompressionHelper.CompressWithGzip(Body);
                Headers["Content-Encoding"] = "gzip";
                Headers["Content-Length"] = bodyBytes.Length.ToString();
                Headers["Vary"] = "Accept-Encoding"; // Best practice with content negotiation
            }
            else
            {
                bodyBytes = Encoding.UTF8.GetBytes(Body);
                Headers["Content-Length"] = bodyBytes.Length.ToString();
            }
        }

        else
        {
            Headers["Content-Length"] = "0";
        }

        // Build the response header string
        var responseBuilder = new StringBuilder();
        responseBuilder.Append($"HTTP/1.1 {StatusCode} {StatusMessage}\r\n");

        foreach (var header in Headers)
        {
            responseBuilder.Append($"{header.Key}: {header.Value}\r\n");
        }

        responseBuilder.Append("\r\n");
        byte[] headerBytes = Encoding.UTF8.GetBytes(responseBuilder.ToString());

        // Combine header and (optionally compressed) body
        if (bodyBytes != null)
        {
            byte[] responseBytes = new byte[headerBytes.Length + bodyBytes.Length];
            Buffer.BlockCopy(headerBytes, 0, responseBytes, 0, headerBytes.Length);
            Buffer.BlockCopy(bodyBytes, 0, responseBytes, headerBytes.Length, bodyBytes.Length);
            return responseBytes;
        }
       
        return headerBytes;
        
    }
}