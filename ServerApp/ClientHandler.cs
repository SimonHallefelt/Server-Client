using System.Data.SqlTypes;
using System.Net.Sockets;
using System.Text;

namespace ServerAPP {
    class ClientHandler {
        TcpClient client;
        NetworkStream stream;
        MessageHandler messageHandler;
        uint clientID;
        

        public ClientHandler(TcpClient client, uint clientID, MessageHandler messageHandler)
        {
            this.client = client;
            this.clientID = clientID;
            this.messageHandler = messageHandler;
            this.stream = this.client.GetStream();

            Console.WriteLine($"client {clientID} connected");
        }

        public async Task handleClient() {
            try {
                await Send($"Hello from Server, to client number {clientID}");

                while (client.Connected) {
                    Console.WriteLine($"Client {clientID} handled on thread {Thread.CurrentThread.ManagedThreadId}");

                    string? received = await Receive();
                    if (received == null) break;
                    Console.WriteLine($"Received from client {clientID}: {received}");
                    string? messageResponse = await messageHandler.handleMessage(clientID, received);
                    if (messageResponse == null) break;
                    await Send(messageResponse);
                }
            } catch (System.Exception e) {
                Console.WriteLine($"Error with client {clientID}: {e}");
            } finally {
                client.Close();
                Console.WriteLine($"client {clientID} disconnected");
            }

        }

        private async Task Send(string massage) {
            byte[] buffer = Encoding.UTF8.GetBytes(massage);
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }
        
        private async Task<string?> Receive() {
            byte[] buffer = new byte[1024];
            try {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) return null;
                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
            catch {
                return null;
            }
        }
    }
}