using System.Net;
using System.Net.Sockets;
using System.Text;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();
var socket = server.AcceptSocket();

var responseBuffer = new byte[1024];
socket.Receive(responseBuffer);

var lines = Encoding.UTF8.GetString(responseBuffer).Split("\r\n");

var line0Parts = lines[0].Split(" ");
var (path, httpVer) = (line0Parts[1], line0Parts[2]);

string response;
if (path.StartsWith("/echo/"))
{
    var bodyContent = path.Remove(0, 6);
    response = $"{httpVer} 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {bodyContent.Length}\r\n\r\n{bodyContent}"; 
}

else if(path.StartsWith('/'))
{
    response = $"{httpVer} 200 OK\r\n\r\n";
}

else
{
    response = $"{httpVer} 404 Not Found\r\n\r\n";

}

socket.Send(Encoding.UTF8.GetBytes(response));