using System.Net;
using System.Net.Sockets;
using CustomHttpServer.RequestHandlers;

namespace CustomHttpServer;

public class HttpServer(IPAddress ipAddress, int port)
{
    private readonly TcpListener _listener = new(ipAddress, port);
    private readonly Dictionary<string, IRequestHandler> _routes = new();
    private bool _isRunning;

    public void AddRoute(string path, IRequestHandler handler)
    {
        _routes[path] = handler;
    }

    public void Start()
    {
        _isRunning = true;
        _listener.Start();
        Console.WriteLine($"Server started and listening on port {((IPEndPoint)_listener.LocalEndpoint).Port}");

        try
        {
            while (_isRunning)
            {
                var client = _listener.AcceptSocket();
                Task.Run(() => ProcessRequest(client));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Server error: {e.Message}");
        }
        finally
        {
            Stop();
        }
    }

    public void Stop()
    {
        _isRunning = false;
        _listener.Stop();
        Console.WriteLine("Server stopped");
    }

    private void ProcessRequest(Socket client)
    {
        try
        {
            using (client)
            {
                while (client.Connected)
                {
                    var request = HttpRequest.Parse(client);
                    if (request == null) break;
                    
                    var response = RouteRequest(request);
                    SendResponse(client, response);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing request: {ex.Message}");
        }
    }
    
    private HttpResponse RouteRequest(HttpRequest request)
    {
        var path = request.Path.Split('/', 3)[1];
        var route = "/" + path;
        
        return 
            _routes.TryGetValue(route, out var handler) ? 
                handler.HandleRequest(request) : new HttpResponse(404);
    }
    
    private void SendResponse(Socket client, HttpResponse response)
    {
        Console.WriteLine("Sending response to client");
        var responseBytes = response.ToByteArray();
        client.Send(responseBytes, SocketFlags.None);
        
        if (response.Headers.TryGetValue("Connection", out string? value) && value == "close")
        {
            client.Disconnect(true);
        }
    }
}