using System.Data.SqlTypes;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace ServerAPP {
    class ClientHandler
    {
        TcpClient client;
        NetworkStream stream;
        MessageHandler messageHandler;
        uint clientID;
        private readonly object streamLock = new();

        public ClientHandler(TcpClient client, uint clientID, MessageHandler messageHandler)
        {
            this.client = client;
            this.clientID = clientID;
            this.messageHandler = messageHandler;
            this.stream = this.client.GetStream();

            Console.WriteLine($"client {clientID} connected");
        }

        public async Task handleClient()
        {
            try
            {
                await Send($"Hello from Server, to client number {clientID}");

                while (client.Connected)
                {
                    Console.WriteLine($"Client {clientID} handled on thread {Thread.CurrentThread.ManagedThreadId}");

                    string? received = await Receive();
                    if (received == null) break;
                    Console.WriteLine($"Received from client {clientID}: {received}");
                    string? messageResponse = await messageHandler.HandleMessage(this, received);
                    if (messageResponse == null) break;
                    await Send(messageResponse);
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"Error with client {clientID}: {e}");
            }
            finally
            {
                client.Close();
                Console.WriteLine($"client {clientID} disconnected");
            }

        }

        public Task Send(string massage)
        {
            lock (streamLock)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(massage);
                stream.WriteAsync(buffer, 0, buffer.Length);
            }
            return Task.CompletedTask;
        }

        private async Task<string?> Receive()
        {
            byte[] buffer = new byte[1024];
            try
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) return null;
                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
            catch
            {
                return null;
            }
        }

        public uint getClientID()
        {
            return clientID;
        }
    }
}