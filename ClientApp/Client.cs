// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;
using System.Text;

namespace ClientApp {
    class Client {
        TcpClient client;

        static void Main(string[] args) {
            new Client();
        }

        public Client() {
            client = new TcpClient();
            client.Connect("127.0.0.1", 5000);
            Console.WriteLine("Client started");

            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            
            Console.WriteLine($"Server says: {response}");
            client.Close();
        }
    }
    
}