using Avalonia.Controls;
using System.Net.Sockets;
using System.Text;
using Avalonia.Threading;
using System.Threading.Tasks;
using System;

namespace ClientApp {
    public partial class MainWindow : Window {
        private TcpClient _client;
        private NetworkStream _stream;

        public MainWindow() {
            InitializeComponent();
            MessageInput.Focus();
            _client = new TcpClient();
            _client.Connect("127.0.0.1", 5000);
            _stream = _client.GetStream();
            Task.Run(ReceiveMessages); // Start listening for server messages
        }

        private async void OnSendButtonClick(object sender, Avalonia.Interactivity.RoutedEventArgs e) {
            string message = MessageInput.Text;
            if (message == null)
            {
                Console.WriteLine("Send: no massage found");
                return;
            }
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await _stream.WriteAsync(buffer, 0, buffer.Length);
        }

        private async Task ReceiveMessages() {
            while (true) {
                byte[] buffer = new byte[1024];
                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) return;
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Dispatcher.UIThread.Post(() => ServerMessages.Text = message); // Update the UI
            }
        }
    }
}
