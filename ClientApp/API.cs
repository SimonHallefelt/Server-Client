using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text;
using System;

namespace ClientApp {
    internal class API {
        private int versionNumber;
        private TcpClient client;
        private NetworkStream stream;

        public API() {
            versionNumber = 0;
            client = new TcpClient();
            client.Connect("127.0.0.1", 5000);
            stream = client.GetStream();
        }

        public async void SendMessage(string message) {
            if (message == null)
            {
                Console.WriteLine("Send: no massage found");
                return;
            }
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }

        public async Task<bool> AttemptLogin() {
            return false;
        }

        public async Task<string> GetMessage() {
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead == 0) return null;
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

    }
}