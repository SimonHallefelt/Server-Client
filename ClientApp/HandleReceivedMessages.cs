using System;
using System.Threading.Tasks;
using SerMesType = SharedLib.SharedLibrary.ServerMessageType;

namespace ClientApp
{
    class HandleReceivedMessages
    {
        API api;
        MainWindow mainWindow;

        public HandleReceivedMessages(API api, MainWindow mainWindow)
        {
            this.api = api;
            this.mainWindow = mainWindow;
            Task.Run(ReceiveMessages); // Start listening for server messages
        }

        private async Task ReceiveMessages()
        {
            while (true)
            {
                (uint versionNumber, string message) = await api.GetMessage();
                Console.WriteLine("client received: " + message);
                if (message == null)
                {
                    Console.WriteLine("message was null");
                    continue;
                }
                string[] data = message.Split(" ");
                if (data.Length < 3)
                {
                    Console.WriteLine("message length was to short");
                    continue;
                }
                bool passed;
                try
                {
                    uint messageVersionNumber = UInt32.Parse(data[0]);
                    if (messageVersionNumber != versionNumber)
                    {
                        Console.WriteLine("error, messageVersionNumber != versionNumber");
                        continue;
                    }
                    SerMesType messageType = Enum.Parse<SerMesType>(data[1]);
                    string[] messageContent = data[2..];
                    passed = await messageTypeSwitch(messageType, messageContent);
                }
                catch (System.Exception)
                {
                    Console.WriteLine("got an error from handling message");
                    continue;
                }
            }
        }

        private async Task<bool> messageTypeSwitch(SerMesType messageType, string[] messageContent)
        {
            bool response = false;
            switch (messageType)
            {
                case SerMesType.DeliverMessage:
                    {
                        response = await deliverMessage(messageContent);
                        break;
                    }
                case SerMesType.DeliverMessages:
                    {
                        response = await deliverMessages(messageContent);
                        break;
                    }
                case SerMesType.DeliverRegisteredAccounts:
                    {
                        response = await deliverRegisteredAccounts(messageContent);
                        break;
                    }
                case SerMesType.AccountRegistrationSuccess:
                    {
                        response = await changeUser(messageContent);
                        break;
                    }
                case SerMesType.LoginSuccess:
                    {
                        response = await changeUser(messageContent);
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

        private async Task<bool> deliverMessage(string[] messageContent)
        {
            await mainWindow.addNewMessage(String.Join(" ", messageContent));
            return true;
        }

        private async Task<bool> deliverMessages(string[] messageContent)
        {
            return false;
        }

        private async Task<bool> changeUser(string[] messageContent)
        {
            if (messageContent[0] == "True")
            {
                mainWindow.setUsername(messageContent[1]);
            }
            return false;
        }

        private async Task<bool> deliverRegisteredAccounts(string[] messageContent)
        {
            await mainWindow.removeAllRegisteredUsers();
            foreach (string user in messageContent)
            {
                await mainWindow.addNewUser(user);
            }
            
            return false;
        }
    }
}
    