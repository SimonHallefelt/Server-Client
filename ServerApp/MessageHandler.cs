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

        public async Task<string> handleMessage(uint clientID, string message)
        {
            try
            {
                string[] data = message.Split(" ");
                if (data.Length < 3) return "error, message data < 3";
                uint messageVersionNumber = UInt32.Parse(data[0]);
                if (messageVersionNumber != versionNumber) return "error, messageVersionNumber != versionNumber";
                CliMesType messageType = Enum.Parse<CliMesType>(data[1]);
                string[] messageContent = data[2..];

                (string, bool) response = await messageTypeSwitch(clientID, messageType, messageContent);

                Console.WriteLine("message response: " + response + " to client: " + clientID);
                response.Item1 = versionNumber + " " + response.Item1;
                return response.Item1;
            }
            catch (System.Exception)
            {
                Console.WriteLine("got an error from handling message");
                return "";
            }
        }

        private async Task<(string, bool)> messageTypeSwitch(uint clientID, CliMesType messageType, string[] messageContent)
        {
            (string, bool) response = ("", false);
            switch (messageType)
            {
                case CliMesType.SendMessage:
                    {
                        response = await sendMessage(clientID, messageContent);
                        break;
                    }
                case CliMesType.AttemptLogin:
                    {
                        response = await attemptLogin(clientID, messageContent);
                        break;
                    }
                case CliMesType.AttemptRegisterAccount:
                    {
                        response = await attemptRegisterAccount(clientID, messageContent);
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

        private Task<(string, bool)> sendMessage(uint clientID, string[] messageContent)
        {
            Console.WriteLine("function sendMessage got: " + messageContent + " from client: " + clientID);

            return Task.FromResult((SerMesType.DeliverMessage + " " + String.Join(" ", messageContent), false));
        }

        private async Task<(string, bool)> attemptLogin(uint clientID, string[] messageContent)
        {
            Console.WriteLine("function attemptLogin got: " + messageContent + " from client: " + clientID);
            if (messageContent.Length < 2)
            {
                return ("missing username or password", false);
            }

            (string, bool) response = await database.login(messageContent[0], messageContent[1]);
            response.Item1 = SerMesType.LoginSuccess + " " + response.Item1;
            return response;
        }

        private async Task<(string, bool)> attemptRegisterAccount(uint clientID, string[] messageContent)
        {
            Console.WriteLine("function attemptRegisterAccount got: " + messageContent + " from client: " + clientID);
            if (messageContent.Length < 2)
            {
                return ("missing username or password", false);
            }

            (string, bool) response = await database.registerAccount(messageContent[0], messageContent[1]);
            response.Item1 = SerMesType.AccountRegistrationSuccess + " " + response.Item1;
            return response;
        }
    }

}