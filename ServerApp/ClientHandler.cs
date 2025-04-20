using System.Net.Sockets;
using System.Text;

namespace ServerAPP {
    class ClientHandler {
        TcpClient client;
        NetworkStream stream;
        int clientID;

        public ClientHandler(TcpClient client, int clientID) {
            Console.WriteLine("client connected");
            this.client = client;
            this.clientID = clientID;
            this.stream = this.client.GetStream();
            send($"Hello from Server, to client number {this.clientID}");
            client.Close();
        }

        private void send(string massage) {
            byte[] buffer = Encoding.UTF8.GetBytes(massage);
            stream.Write(buffer, 0, buffer.Length);

        }
        
        private string receive() {
            return "";
        }
    }
}