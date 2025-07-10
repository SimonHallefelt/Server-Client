using MesType = SharedLib.SharedLibrary.MessageType;

namespace ServerAPP
{
    class MessageHandler
    {
        private uint versionNumber = 0;

        public MessageHandler()
        {

        }

        public async Task<string> handleMessage(uint clientID, string message)
        {
            string[] data = message.Split(" ");
            if (data.Length < 3) return "error, message data < 3";
            uint messageVersionNumber = UInt32.Parse(data[0]);
            if (messageVersionNumber != versionNumber) return "error, messageVersionNumber != versionNumber";
            MesType messageType = Enum.Parse<MesType>(data[1]);
            string[] messageContent = data[2..];

            (string, bool) response = await messageTypeSwitch(clientID, messageType, messageContent);

            return response.Item1;
        }

        private async Task<(string, bool)> messageTypeSwitch(uint clientID, MesType messageType, string[] messageContent)
        {
            (string, bool) response = ("", false);
            switch (messageType)
            {
                case MesType.SendMessage:
                    {
                        response = await sendMessage(clientID, messageContent);
                        break;
                    }
                case MesType.AttemptLogin:
                    {
                        response = await attemptLogin(clientID, messageContent);
                        break;
                    }
                case MesType.AttemptRegisterAccount:
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

            return Task.FromResult((String.Join(" ", messageContent), false));
        }

        private Task<(string, bool)> attemptLogin(uint clientID, string[] messageContent)
        {
            Console.WriteLine("function attemptLogin got: " + messageContent + " from client: " + clientID);

            return Task.FromResult((String.Join(" ", messageContent), false));
        }

        private Task<(string, bool)> attemptRegisterAccount(uint clientID, string[] messageContent)
        {
            Console.WriteLine("function attemptRegisterAccount got: " + messageContent + " from client: " + clientID);

            return Task.FromResult((String.Join(" ", messageContent), false));
        }
    }

}