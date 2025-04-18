// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerAPP {
    class Server
    {
        TcpListener server;

        static void Main(string[] args) {
            new Server();
        }

        public Server() {
            server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            Console.WriteLine("Started the Server");
            Run();
        }

        private void Run() {
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("client connected");

            NetworkStream stream = client.GetStream();
            byte[] buffer = Encoding.UTF8.GetBytes("Hello from Server");
            stream.Write(buffer, 0, buffer.Length);

            client.Close();
        }
    }
}
