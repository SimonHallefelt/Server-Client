using System.Net;
using CliMesType = SharedLib.SharedLibrary.ClientMessageType;
using SerMesType = SharedLib.SharedLibrary.ServerMessageType;

namespace ServerAPP
{
    class MessageHandler
    {
        private uint versionNumber = 0;
        private Database database;

        public MessageHandler()
        {
            this.database = new Database();
        }

        public async Task<string> HandleMessage(ClientHandler client, string message)
        {
            try
            {
                string[] data = message.Split(" ");
                if (data.Length < 3) return "error, message data < 3";
                uint messageVersionNumber = UInt32.Parse(data[0]);
                if (messageVersionNumber != versionNumber) return "error, messageVersionNumber != versionNumber";
                CliMesType messageType = Enum.Parse<CliMesType>(data[1]);
                string[] messageContent = data[2..];

                (string, bool) response = await MessageTypeSwitch(client, messageType, messageContent);

                Console.WriteLine("message response: " + response + " to client: " + client.getClientID());
                response.Item1 = versionNumber + " " + response.Item1;
                return response.Item1;
            }
            catch (System.Exception)
            {
                Console.WriteLine("got an error from handling message");
                return "";
            }
        }

        private async Task<(string, bool)> MessageTypeSwitch(ClientHandler client, CliMesType messageType, string[] messageContent)
        {
            (string, bool) response = ("", false);
            switch (messageType)
            {
                case CliMesType.SendMessage:
                    {
                        response = await SendMessage(client, messageContent);
                        break;
                    }
                case CliMesType.AttemptLogin:
                    {
                        response = await AttemptLogin(client, messageContent);
                        break;
                    }
                case CliMesType.AttemptRegisterAccount:
                    {
                        response = await AttemptRegisterAccount(client, messageContent);
                        break;
                    }
                case CliMesType.RequestChatLogFor:
                    {
                        response = await RequestChatLogFor(client, messageContent);
                        break;
                    }
                case CliMesType.RequestUpdateFromServer:
                    {
                        response = await RequestUpdateFromServer(client, messageContent);
                        break;
                    }
                default:
                    {
                        Console.WriteLine("has not implemented a function for: " + messageType);
                        break;
                    }
            }

            return response;
        }

        private async Task<(string, bool)> SendMessage(ClientHandler client, string[] messageContent)
        {
            Console.WriteLine("function sendMessage got: " + string.Join(" ", messageContent) + " from client: " + client.getClientID());
            if (messageContent.Length < 3)
            {
                return ("error: SendMessage missing a user or the message", false);
            }
            string user1 = messageContent[0];
            string user2 = messageContent[1];
            string[] messageText = messageContent[2..];
            var response = await database.addMessage(user1, user2, messageText);

            return (SerMesType.DeliverMessage + " " + user1 + " " + user2 + " " + response.Item1, response.Item2);
        }

        private async Task<(string, bool)> AttemptLogin(ClientHandler client, string[] messageContent)
        {
            Console.WriteLine("function attemptLogin got: " + string.Join(" ", messageContent) + " from client: " + client.getClientID());
            if (messageContent.Length < 2)
            {
                return ("missing username or password", false);
            }

            (string, bool) response = await database.Login(messageContent[0], messageContent[1]);
            if (response.Item2)
            {
                _ = Task.Run(() => DeliverRegisteredAccounts(client, messageContent[0], null));
            }
            response.Item1 = SerMesType.LoginSuccess + " " + response.Item2 + " " + messageContent[0] + " " + response.Item1;
            return response;
        }

        private async Task<(string, bool)> AttemptRegisterAccount(ClientHandler client, string[] messageContent)
        {
            Console.WriteLine("function attemptRegisterAccount got: " + string.Join(" ", messageContent) + " from client: " + client.getClientID());
            if (messageContent.Length < 2)
            {
                return ("missing username or password", false);
            }

            (string, bool) response = await database.RegisterAccount(messageContent[0], messageContent[1]);
            if (response.Item2)
            {
                _ = Task.Run(() => DeliverRegisteredAccounts(client, messageContent[0], null));
            }
            response.Item1 = SerMesType.AccountRegistrationSuccess + " " + response.Item2 + " " + messageContent[0] + " " + response.Item1;
            return response;
        }

        private async Task<(string, bool)> RequestChatLogFor(ClientHandler client, string[] messageContent)
        {
            Console.WriteLine("function RequestChatLogFor got: " + string.Join(" ", messageContent) + " from client: " + client.getClientID());
            if (messageContent.Length < 2)
            {
                return ("missing a username", false);
            }

            (string, bool) response = await database.GetChatLogsFor(messageContent[0], messageContent[1]);

            response.Item1 = SerMesType.DeliverMessages + " " + response.Item2 + " " + messageContent[0] + " " + messageContent[1] + " " + response.Item1;
            return response;
        }

        private async Task<(string, bool)> RequestUpdateFromServer(ClientHandler client, string[] messageContent)
        {
            Console.WriteLine("function RequestUpdateFromServer got: " + string.Join(" ", messageContent) + " from client: " + client.getClientID());
            if (messageContent.Length < 3)
            {
                return ("missing a data", false);
            }
            int i = 0;
            DateTime? UpdatedRegisteredAccounts = null;
            DateTime? UpdatedMessages = null;
            if (messageContent[i] == null || messageContent[i] == "")
                i++;
            else
            {
                UpdatedRegisteredAccounts = DateTime.Parse(string.Join(" ", messageContent[0..2]));
                i += 2;
            }
            if (messageContent[i] == null || messageContent[i] == "")
                i++;
            else
            {
                UpdatedMessages = DateTime.Parse(string.Join(" ", messageContent[i..(i + 2)]));
                i += 2;
            }
            string sender = messageContent[i];
            _ = Task.Run(() => DeliverRegisteredAccounts(client, sender, UpdatedRegisteredAccounts));
            if (i + 1 < messageContent.Length)
            {
                string receiver = messageContent[i + 1];
                _ = Task.Run(async () => await GetLatestChatLogsFor(client, sender, receiver, UpdatedMessages));
            }

            return ("RequestUpdateFromServer was successful", true);
        }

        private async Task GetLatestChatLogsFor(ClientHandler client, string sender, string receiver, DateTime? UpdatedMessages)
        {
            if (UpdatedMessages != null)
            {
                (string, bool) response = await database.GetNewChatLogsFor(sender, receiver, (DateTime)UpdatedMessages);
                if (!response.Item2)
                    return;
                string message = response.Item2 + " " + sender + " " + receiver + " " + response.Item1;
                await SendToClient(client, SerMesType.DeliverMessages, message);
            }
        }

        private async Task DeliverRegisteredAccounts(ClientHandler client, string username, DateTime? UpdatedRegisteredAccountsAt)
        {
            Console.WriteLine("deliverRegisteredAccounts to client: " + client.getClientID());
            try
            {
                if (UpdatedRegisteredAccountsAt != null && UpdatedRegisteredAccountsAt > database.getLatestAccountRegisteredAt())
                    return;

                string[] registeredAccounts = await database.GetAllOtherRegisteredAccounts(username);
                if (registeredAccounts.Length == 0)
                {
                    Console.WriteLine("deliverRegisteredAccounts to client is empty: " + client.getClientID());
                    return;
                }
                await SendToClient(client, SerMesType.DeliverRegisteredAccounts, String.Join(" ", registeredAccounts));
            }
            catch (System.Exception)
            {
                Console.WriteLine("error: deliverRegisteredAccounts to client: " + client.getClientID());
                return;
            }
        }

        private async Task SendToClient(ClientHandler client, SerMesType smt, string message)
        {
            message = versionNumber + " " + smt + " " + message;
            await client.Send(message);
        }
    }

}