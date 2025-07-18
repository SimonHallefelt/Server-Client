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
        private volatile string username = null;
        private volatile string otherUser = null;

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
            if (username != null && otherUser != null)
                api.SendMessage(username + " " + otherUser + " " + message);
            else
                Console.WriteLine("cant send message, is not login or missing a receiver");
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

        private async void OnUserClicked(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // switch to that users chat
            if (sender is Button button)
            {
                string otherUser = button.Content.ToString();
                this.otherUser = otherUser;
                MessageContainer.Children.Clear();
                Console.WriteLine("onUserClicked, user that was clicked is: " + otherUser);
                await api.RequestChatLogFor(this.username, button.Content + "");
            }
        }

        public void SetUsername(string username)
        {
            Console.WriteLine("Change user to: " + username);
            this.username = username;
            this.otherUser = null;
            Dispatcher.UIThread.Post(() =>
            {
                if (CurrentUser != null)
                    CurrentUser.Text = $"User: {username}";
                MessageContainer.Children.Clear();
            });
        }

        public Task<bool> AddNewMessage(string message)
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

        public Task<bool> AddNewUser(string username)
        {
            Dispatcher.UIThread.Post(() =>
            {
                var newUser = new Border
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(2),
                    CornerRadius = new CornerRadius(5),
                    Margin = new Thickness(10),

                };

                var button = new Button
                {
                    Content = username,
                    Margin = new Thickness(10),
                };
                button.Click += OnUserClicked;

                newUser.Child = button;
                UserContainer.Children.Add(newUser);

                UserScroll.ScrollToEnd();
            }); // Update the UI with a new message
            return Task.FromResult(true);
        }

        public Task RemoveAllRegisteredUsers()
        {
            Dispatcher.UIThread.Post(() =>
            {
                UserContainer.Children.Clear();
            });
            return Task.CompletedTask;
        }

        public Task RemoveAllMessages()
        {
            Dispatcher.UIThread.Post(() =>
            {
                MessageContainer.Children.Clear();
            });
            return Task.CompletedTask;
        }

        public string getUsername()
        {
            return username;
        }

        public string getOtherUser()
        {
            return otherUser;
        }
    }
}
