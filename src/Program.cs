using System.Net;
using CustomHttpServer.RequestHandlers;

namespace CustomHttpServer;

public class Program
{
    private static void Main(string[] args)
    {
        string directory = string.Empty;
        
        if (args.Length > 1)
        {
            directory = args[1];
        }
        
        var server = new HttpServer(IPAddress.Any, 4221);
        
        server.AddRoute("/", new RootHandler());
        server.AddRoute("/echo", new EchoHandler());
        server.AddRoute("/user-agent", new UserAgentHandler());
        server.AddRoute("/files", new FileHandler(directory));

        server.Start();
    }
}