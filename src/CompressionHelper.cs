using System.IO.Compression;
using System.Text;

namespace CustomHttpServer;

public static class CompressionHelper
{
    public static byte[] CompressWithGzip(string data)
    {
        using var memoryStream = new MemoryStream();
        using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
        {
            // Convert string to bytes first to avoid any encoding issues/BOMs
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            gzipStream.Write(bytes, 0, bytes.Length);
        }

        return memoryStream.ToArray();
    }

    public static bool ClientSupportsGzip(HttpRequest request)
    {
        if (request.Headers.TryGetValue("Accept-Encoding", out var encodings))
        {
            return encodings.Split(',')
                .Select(e => e.Trim().ToLowerInvariant())
                .Contains("gzip");
        }
        return false;
    }
}