using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerAPP {
    class Server
    {
        TcpListener server;
        List<ClientHandler> clientHandlers;
        int numberOfClients;

        static async Task Main(string[] args) {
            await new Server().run();
        }

        public Server() {
            server = new TcpListener(IPAddress.Any, 5000);
            numberOfClients = 0;
            clientHandlers = new List<ClientHandler>();

            server.Start();
            Console.WriteLine("Started the Server");
        }

        private async Task run() {
            while (true) {
                TcpClient client = await server.AcceptTcpClientAsync();
                ClientHandler clientHandler = new ClientHandler(client, numberOfClients);
                numberOfClients++;
                lock (clientHandlers) {
                    clientHandlers.Append(clientHandler);
                }
                clientHandler.handleClient();
            }
        }
    }
}
