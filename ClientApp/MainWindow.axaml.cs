using Avalonia.Controls;
using Avalonia;
using System.Net.Sockets;
using System.Text;
using Avalonia.Threading;
using System.Threading.Tasks;
using System;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace ClientApp {
    public partial class MainWindow : Window {
        private API api;

        public MainWindow() {
            InitializeComponent();
            MessageInput.Focus();
            api = new API();
            Task.Run(ReceiveMessages); // Start listening for server messages
        }

        private void OnSendButtonClick(object sender, Avalonia.Interactivity.RoutedEventArgs e) {
            string message = MessageInput.Text;
            api.SendMessage(message);
            MessageInput.Text = "";
        }

        private async void OnLoginClick(object sender, Avalonia.Interactivity.RoutedEventArgs e) {
            string username = Username.Text;
            string password = Password.Text;
            await api.AttemptLogin(username, password);
        }

        private async void OnRegisterClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string username = Username.Text;
            string password = Password.Text;
            await api.AttemptRegisterAccount(username, password);
        }

        private async Task ReceiveMessages() {
            while (true) {
                string message = await api.GetMessage();
                if (message == null) continue;
                Dispatcher.UIThread.Post(() =>
                {
                    var newMessage = new Border
                    {
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(2),
                        CornerRadius = new CornerRadius(5),
                        Margin = new Thickness(10),
                        Child = new TextBlock { 
                            Text = message,
                            Margin = new Thickness(10),
                            TextWrapping = TextWrapping.Wrap
                        }
                    };

                    MessageContainer.Children.Add(newMessage);
                
                    MessageScroll.ScrollToEnd();
                }); // Update the UI whith a new message
            }
        }
    }
}
