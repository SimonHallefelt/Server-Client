using System.Net.Sockets;
using System.Text;

namespace ServerAPP {
    class ClientHandler {
        TcpClient client;
        NetworkStream stream;
        int clientID;

        public ClientHandler(TcpClient client, int clientID) {
            this.client = client;
            this.clientID = clientID;
            this.stream = this.client.GetStream();

            Console.WriteLine($"client {clientID} connected");
        }

        public async void handleClient() {
            try {
                await Send($"Hello from Server, to client number {clientID}");

                while (client.Connected) {
                    string? message = await Receive();
                    if (message == null) break;
                    Console.WriteLine($"Received from client {clientID}: {message}");
                    Send(message);
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