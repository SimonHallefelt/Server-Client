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

        public async Task<string> handleMessage(ClientHandler client, string message)
        {
            try
            {
                string[] data = message.Split(" ");
                if (data.Length < 3) return "error, message data < 3";
                uint messageVersionNumber = UInt32.Parse(data[0]);
                if (messageVersionNumber != versionNumber) return "error, messageVersionNumber != versionNumber";
                CliMesType messageType = Enum.Parse<CliMesType>(data[1]);
                string[] messageContent = data[2..];

                (string, bool) response = await messageTypeSwitch(client, messageType, messageContent);

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

        private async Task<(string, bool)> messageTypeSwitch(ClientHandler client, CliMesType messageType, string[] messageContent)
        {
            (string, bool) response = ("", false);
            switch (messageType)
            {
                case CliMesType.SendMessage:
                    {
                        response = await sendMessage(client, messageContent);
                        break;
                    }
                case CliMesType.AttemptLogin:
                    {
                        response = await attemptLogin(client, messageContent);
                        break;
                    }
                case CliMesType.AttemptRegisterAccount:
                    {
                        response = await attemptRegisterAccount(client, messageContent);
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

        private Task<(string, bool)> sendMessage(ClientHandler client, string[] messageContent)
        {
            Console.WriteLine("function sendMessage got: " + messageContent + " from client: " + client.getClientID());

            return Task.FromResult((SerMesType.DeliverMessage + " " + String.Join(" ", messageContent), false));
        }

        private async Task<(string, bool)> attemptLogin(ClientHandler client, string[] messageContent)
        {
            Console.WriteLine("function attemptLogin got: " + messageContent + " from client: " + client.getClientID());
            if (messageContent.Length < 2)
            {
                return ("missing username or password", false);
            }

            (string, bool) response = await database.login(messageContent[0], messageContent[1]);
            if (response.Item2)
            {
                _ = Task.Run(() => deliverRegisteredAccounts(client, messageContent[0]));
            }
            response.Item1 = SerMesType.LoginSuccess + " " + response.Item1;
            return response;
        }

        private async Task<(string, bool)> attemptRegisterAccount(ClientHandler client, string[] messageContent)
        {
            Console.WriteLine("function attemptRegisterAccount got: " + messageContent + " from client: " + client.getClientID());
            if (messageContent.Length < 2)
            {
                return ("missing username or password", false);
            }

            (string, bool) response = await database.registerAccount(messageContent[0], messageContent[1]);
            if (response.Item2)
            {
                _ = Task.Run(() => deliverRegisteredAccounts(client, messageContent[0]));
            }
            response.Item1 = SerMesType.AccountRegistrationSuccess + " " + response.Item1;
            return response;
        }

        private async Task deliverRegisteredAccounts(ClientHandler client, string username)
        {
            Console.WriteLine("deliverRegisteredAccounts to client: " + client.getClientID());
            try
            {
                string[] registeredAccounts = await database.getAllOtherRegisteredAccounts(username);
                if (registeredAccounts.Length == 0)
                {
                    Console.WriteLine("error: deliverRegisteredAccounts to client is empty: " + client.getClientID());
                    return;
                }
                string message = versionNumber + " " + SerMesType.DeliverRegisteredAccounts + " " + String.Join(" ", registeredAccounts);
                await client.Send(message);
            }
            catch (System.Exception)
            {
                Console.WriteLine("error: deliverRegisteredAccounts to client: " + client.getClientID());
                return;
            }
        }
    }

}