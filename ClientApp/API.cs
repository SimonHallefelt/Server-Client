using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text;
using System;
using MesType = SharedLib.SharedLibrary.MessageType;

namespace ClientApp {
    internal class API {
        private uint versionNumber;
        private TcpClient client;
        private NetworkStream stream;

        public API() {
            versionNumber = 0;
            client = new TcpClient();
            client.Connect("127.0.0.1", 5000);
            stream = client.GetStream();
        }

        public async void SendMessage(string message) {
            if (message == null) {
                Console.WriteLine("Send: no massage found");
                return;
            }
            await SendToServer(message, MesType.SendMessage);
        }

        public async Task<bool> AttemptLogin(string username, string password) {
            if (username == null || password == null) {
                Console.WriteLine("Send: no password or username found");
                return false;
            }
            if (username.Contains(" ") || password.Contains(" ")) {
                Console.WriteLine("Send: password or username contained space");
                return false;
            }
            string message = username + " " + password;
            await SendToServer(message, MesType.AttemptLogin);
            return true;
        }

        public async Task<bool> AttemptRegisterAccount(string username, string password) {
            if (username == null || password == null) {
                Console.WriteLine("Send: no password or username found");
                return false;
            }
            if (username.Contains(" ") || password.Contains(" ")) {
                Console.WriteLine("Send: password or username contained space");
                return false;
            }
            string message = username + " " + password;
            await SendToServer(message, MesType.AttemptRegisterAccount);
            return true;
        }

        private async Task<bool> SendToServer(string message, MesType mt)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(versionNumber + " " + mt + " " + message);
            await stream.WriteAsync(buffer, 0, buffer.Length);
            return true;
        }

        public async Task<string> GetMessage()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead == 0) return null;
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

    }
}