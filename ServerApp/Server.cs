using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerAPP {
    class Server
    {
        TcpListener server;
        List<ClientHandler> clientHandlers;
        int numberOfClients;

        static void Main(string[] args) {
            new Server();
        }

        public Server() {
            server = new TcpListener(IPAddress.Any, 5000);
            numberOfClients = 0;
            clientHandlers = new List<ClientHandler>();

            server.Start();
            Console.WriteLine("Started the Server");
            Run();
        }

        private void Run() {
            while (true) {
                ClientHandler client = new ClientHandler(server.AcceptTcpClient(), numberOfClients);
                numberOfClients++;
                clientHandlers.Append(client);
            }
        }
    }
}
