using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerAPP {
    class Server
    {
        TcpListener server;
        List<ClientHandler> clientHandlers;
        MessageHandler messageHandler;
        int numberOfClients;

        static async Task Main(string[] args) {
            await new Server().run();
        }

        public Server() {
            server = new TcpListener(IPAddress.Any, 5000);
            numberOfClients = 0;
            clientHandlers = new List<ClientHandler>();
            messageHandler = new MessageHandler();

            server.Start();
            Console.WriteLine("Started the Server");
        }

        private async Task run() {
            while (true) {
                TcpClient client = await server.AcceptTcpClientAsync();
                ClientHandler clientHandler = new ClientHandler(client, numberOfClients, messageHandler);
                numberOfClients++;
                lock (clientHandlers) {
                    clientHandlers.Append(clientHandler);
                }
                _ = Task.Run(() => clientHandler.handleClient());
            }
        }
    }
}
