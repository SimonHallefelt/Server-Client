using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text;
using System;
using CliMesType = SharedLib.SharedLibrary.ClientMessageType;

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
            await SendToServer(message, CliMesType.SendMessage);
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
            await SendToServer(message, CliMesType.AttemptLogin);
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
            await SendToServer(message, CliMesType.AttemptRegisterAccount);
            return true;
        }

        public async Task<bool> RequestChatLogFor(string username1, string username2) {
            if (username1 == null || username2 == null) {
                Console.WriteLine("Send: a username not found");
                return false;
            }
            if (username1.Contains(" ") || username2.Contains(" ")) {
                Console.WriteLine("Send: a username contained space");
                return false;
            }
            string message = username1 + " " + username2;
            await SendToServer(message, CliMesType.RequestChatLogFor);
            return true;
        }

        private async Task<bool> SendToServer(string message, CliMesType mt)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(versionNumber + " " + mt + " " + message);
            await stream.WriteAsync(buffer, 0, buffer.Length);
            return true;
        }

        public async Task<(uint, string)> GetMessage()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead == 0) return (versionNumber, null);
            return (versionNumber, Encoding.UTF8.GetString(buffer, 0, bytesRead));
        }

    }
}