// using System.Net.Sockets;
// using System.Text;
// using System.Threading.Tasks;

// namespace ClientApp {
//     class Client {
//         TcpClient client;
//         NetworkStream stream;

//         static async Task Main(string[] args) {
//             await new Client().run();
//         }

//         public Client() {
//             client = new TcpClient();
//             client.Connect("127.0.0.1", 5000);
//             stream = client.GetStream();

//             Console.WriteLine("Client started");
//         }

//         private async Task run() {
//             _ = HandleServerMassages();
//             await GUI();
//         }

//         private async Task GUI() {
//             await Task.Delay(10000);
//         }

//         private void Send(string massage) {
//             byte[] buffer = Encoding.UTF8.GetBytes(massage);
//             stream.Write(buffer, 0, buffer.Length);
//         }

//         private async Task HandleServerMassages() {
//             while (true) {
//                 string? massage = await Receive();
//                 if (massage == null) {
//                     Console.WriteLine("Server connection lost");
//                     client.Close();
//                     Environment.Exit(1);
//                 }
//                 Console.WriteLine($"server massage: {massage}");
//             }
//         }
        
//         private async Task<string?> Receive() {
//             byte[] buffer = new byte[1024];
//             try {
//                 int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
//                 if (bytesRead == 0) return null;
//                 return Encoding.UTF8.GetString(buffer, 0, bytesRead);
//             } catch {
//                 return null;
//             }
//         }
//     }
    
// }