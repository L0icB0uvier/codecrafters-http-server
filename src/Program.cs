using System.Net;
using CustomHttpServer.RequestHandlers;

namespace CustomHttpServer;

public class Program
{
    private static void Main(string[] args)
    {
        var server = new HttpServer(IPAddress.Any, 4221);

        server.AddRoute("/", new RootHandler());
        server.AddRoute("/echo", new EchoHandler());
        server.AddRoute("/user-agent", new UserAgentHandler());

        server.Start();
    }
}