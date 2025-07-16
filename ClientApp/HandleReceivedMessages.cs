using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SerMesType = SharedLib.SharedLibrary.ServerMessageType;
using Message = SharedLib.SharedLibrary.Message;
using System.Linq;

namespace ClientApp
{
    class HandleReceivedMessages
    {
        API api;
        MainWindow mainWindow;
        List<Message> messages = null;

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
                        response = await DeliverMessage(messageContent);
                        break;
                    }
                case SerMesType.DeliverMessages:
                    {
                        response = await DeliverMessages(messageContent);
                        break;
                    }
                case SerMesType.DeliverRegisteredAccounts:
                    {
                        response = await DeliverRegisteredAccounts(messageContent);
                        break;
                    }
                case SerMesType.AccountRegistrationSuccess:
                    {
                        response = await ChangeUser(messageContent);
                        break;
                    }
                case SerMesType.LoginSuccess:
                    {
                        response = await ChangeUser(messageContent);
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

        private async Task<bool> DeliverMessage(string[] messageContent)
        {
            if (messageContent[0] == mainWindow.getUsername() && messageContent[1] == mainWindow.getOtherUser())
            {
                if (messages == null)
                    messages = ParesMessages(messageContent[2..]);
                else
                    messages.Add(ParesMessages(messageContent[2..])[0]);
                Message message = messages.Last();
                await mainWindow.AddNewMessage(message.GetMessageContent());
                return true;
            }
            return false;
        }

        private async Task<bool> DeliverMessages(string[] messageContent)
        {
            if (messageContent[0] == "True" && messageContent[1] == mainWindow.getUsername() && messageContent[2] == mainWindow.getOtherUser())
            {
                this.messages = ParesMessages(messageContent[3..]);
                foreach (var message in messages)
                {
                    await mainWindow.AddNewMessage(message.GetMessageContent());
                }
                return true;
            }
            return false;
        }

        private async Task<bool> ChangeUser(string[] messageContent)
        {
            if (messageContent[0] == "True")
            {
                mainWindow.SetUsername(messageContent[1]);
            }
            return false;
        }

        private async Task<bool> DeliverRegisteredAccounts(string[] messageContent)
        {
            await mainWindow.RemoveAllRegisteredUsers();
            foreach (string user in messageContent)
            {
                await mainWindow.AddNewUser(user);
            }

            return false;
        }


        public List<Message> ParesMessages(string[] messagesString)
        {
            Console.WriteLine("test "+string.Join(" ", messagesString));
            int i = 3;
            List<Message> messages = new List<Message>();
            try
            {
                while (i < messagesString.Length)
                {
                    int wordCount = int.Parse(messagesString[i]);

                    messages.Add(ParesMessage(messagesString[(i - 3)..(i + wordCount + 1)]));
                    i += wordCount + 4;
                }

            }
            catch (System.Exception)
            {
                Console.WriteLine("could not pares all messages");
            }
            return messages;
        }

        public Message ParesMessage(string[] message)
        {
            string sender = message[0];
            DateTime dateTime = DateTime.Parse(string.Join(" ", message[1..3]));
            int wordCount = int.Parse(message[3]);
            string[] messageText = message[4..];
            return new Message(sender, dateTime, messageText);
        }
    }
}
    