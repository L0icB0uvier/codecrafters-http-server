using System.Net;
using System.Net.Sockets;
using System.Text;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();

while (true)
{
    var socket = await server.AcceptSocketAsync();

    _ = Task.Run(async () =>
    {
        try
        {
            var buffer = new byte[1024];
            await socket.ReceiveAsync(buffer, SocketFlags.None);

            var lines = Encoding.UTF8.GetString(buffer).Split("\r\n");

            foreach(var line in lines)
            {
                Console.WriteLine(line);
            }

            var line0Parts = lines[0].Split(" ");
            var (path, httpVer) = (line0Parts[1], line0Parts[2]);

            string response;

            if(path.StartsWith("/user-agent")){
                var userAgent = lines[2].Split(": ")[1];
                response = $"{httpVer} 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {userAgent.Length}\r\n\r\n{userAgent}"; 
            }

            else if (path.StartsWith("/echo/"))
            {
                var bodyContent = path.Remove(0, 6);
                response = $"{httpVer} 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {bodyContent.Length}\r\n\r\n{bodyContent}"; 
            }

            else if(path.EndsWith('/'))
            {
                response = $"{httpVer} 200 OK\r\n\r\n";
            }

            else
            {
                response = $"{httpVer} 404 Not Found\r\n\r\n";
            }

            await socket.SendAsync(Encoding.UTF8.GetBytes(response), SocketFlags.None);
        }

        catch(Exception e){
            Console.WriteLine(e.Message);
        }
        
        finally
        {
            socket.Close();
        }
        
    });  
}