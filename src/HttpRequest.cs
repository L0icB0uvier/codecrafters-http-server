using System.Net.Sockets;
using System.Text;

namespace CustomHttpServer;

public class HttpRequest
{
    public string Method { get; set; }
    public string Path { get; set; }
    public string HttpVersion { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public string Body { get; set; }
    
    public static HttpRequest? Parse(Socket client)
    {
        var buffer = new byte[1024];
        int bytesRead = client.Receive(buffer, SocketFlags.None);

        if (bytesRead == 0)
            return null;

        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        string[] lines = message.Split("\r\n");

        if (lines.Length == 0)
            return null;

        // Parse request line
        string[] requestParts = lines[0].Split(' ');
        if (requestParts.Length < 3)
            return null;

        var request = new HttpRequest
        {
            Method = requestParts[0],
            Path = requestParts[1],
            HttpVersion = requestParts[2]
        };

        // Parse headers
        int i = 1;
        while (i < lines.Length && !string.IsNullOrEmpty(lines[i]))
        {
            string[] headerParts = lines[i].Split(": ", 2);
            if (headerParts.Length == 2)
            {
                request.Headers[headerParts[0]] = headerParts[1];
            }
            i++;
        }

        // Parse body if present
        if (i < lines.Length - 1)
        {
            request.Body = string.Join("\r\n", lines.Skip(i + 1));
        }

        return request;
    }
}