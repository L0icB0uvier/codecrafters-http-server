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

var response = path == "/" ? 
    $"{httpVer} 200 OK\r\n\r\n" : 
    $"{httpVer} 404 Not Found\r\n\r\n";

socket.Send(Encoding.UTF8.GetBytes(response));