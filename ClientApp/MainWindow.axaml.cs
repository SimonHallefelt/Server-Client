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
    public partial class MainWindow : Window
    {
        private API api;

        public MainWindow()
        {
            InitializeComponent();
            MessageInput.Focus();
            api = new API();
            HandleReceivedMessages hrm = new HandleReceivedMessages(api, this); // handel all messages from server
        }

        private void OnSendButtonClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            string message = MessageInput.Text;
            api.SendMessage(message);
            MessageInput.Text = "";
        }

        private async void OnLoginClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
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

        public Task<bool> addNewMessage(string message)
        {
            Dispatcher.UIThread.Post(() =>
            {
                var newMessage = new Border
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(2),
                    CornerRadius = new CornerRadius(5),
                    Margin = new Thickness(10),
                    Child = new TextBlock
                    {
                        Text = message,
                        Margin = new Thickness(10),
                        TextWrapping = TextWrapping.Wrap
                    }
                };

                MessageContainer.Children.Add(newMessage);

                MessageScroll.ScrollToEnd();
            }); // Update the UI whith a new message
            return Task.FromResult(true);
        }
        
        public Task<bool> addNewUser(string username)
        {
            Dispatcher.UIThread.Post(() =>
            {
                var newMessage = new Border
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(2),
                    CornerRadius = new CornerRadius(5),
                    Margin = new Thickness(10),
                    Child = new Button
                    {
                        Content = username,
                        Margin = new Thickness(10),
                    }
                };

                UserContainer.Children.Add(newMessage);

                UserScroll.ScrollToEnd();
            }); // Update the UI whith a new message
            return Task.FromResult(true);
        }
    }
}
