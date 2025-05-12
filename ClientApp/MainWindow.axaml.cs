using Avalonia.Controls;
using System.Net.Sockets;
using System.Text;
using Avalonia.Threading;
using System.Threading.Tasks;
using System;

namespace ClientApp {
    public partial class MainWindow : Window {
        private NetworkStream _stream;
        private API api;

        public MainWindow() {
            InitializeComponent();
            MessageInput.Focus();
            TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", 5000);
            NetworkStream _stream = client.GetStream();
            api = new API();
            Task.Run(ReceiveMessages); // Start listening for server messages
        }

        private void OnSendButtonClick(object sender, Avalonia.Interactivity.RoutedEventArgs e) {
            string message = MessageInput.Text;
            api.SendMessage(message);
            MessageInput.Text = "";
        }

        private async void OnLoginClick(object sender, Avalonia.Interactivity.RoutedEventArgs e) {
            string userName = UserName.Text;
            string password = Password.Text;
            await api.AttemptLogin();
        }

        private async Task ReceiveMessages() {
            while (true) {
                string message = await api.GetMessage();
                if (message == null) continue;
                Dispatcher.UIThread.Post(() => ServerMessages.Text = message); // Update the UI
            }
        }
    }
}
