using System.Collections.Concurrent;

namespace ServerAPP
{
    class Database
    {
        private ConcurrentDictionary<string, string> loginInfo;
        private ConcurrentDictionary<string, List<Message>> chats;

        public Database()
        {
            this.loginInfo = new ConcurrentDictionary<string, string>();
            this.chats = new ConcurrentDictionary<string, List<Message>>();
        }

        public Task<(string, bool)> addMessage(string user1, string user2, string[] messageContent)
        {
            try
            {
                Message message = new Message(messageContent);
                string key = user1 + " " + user2;
                if (chats.TryGetValue(key, out var list))
                {
                    list.Add(message);
                    return Task.FromResult(("added message to the list", true));
                } else
                {
                    chats.TryAdd(key, [message]);
                    return Task.FromResult(("made a new message list", true));
                }
            }
            catch (System.Exception)
            {
                return Task.FromResult(("error: could not addMessage", false));
            }
        }

        public Task<(string, bool)> RegisterAccount(string username, string password)
        {
            bool added = loginInfo.TryAdd(username, password);
            return Task.FromResult(added ? ("User registered", true) : ("Username already taken", false));
        }

        public Task<(string, bool)> Login(string username, string password)
        {
            bool valid = loginInfo.TryGetValue(username, out var storedPassword) && storedPassword == password;
            return Task.FromResult(valid ? ("Login successful", true) : ("Invalid username or password", false));
        }

        public Task<string[]> GetAllOtherRegisteredAccounts(string account)
        {
            var allAccounts = loginInfo.Keys.ToList();
            allAccounts.Remove(account);
            return Task.FromResult(allAccounts.ToArray());
        }

        public Task<(string, bool)> GetChatLogsFor(string user1, string user2)
        {
            string key = user1 + " " + user2;
            if (chats.TryGetValue(key, out var list))
            {
                string messagesString = "";
                foreach (Message message in list)
                {
                    messagesString += message.ToString();
                }
                return Task.FromResult((messagesString, true));
            }
            return Task.FromResult(("", false));
        }

    }

    class Message
    {
        string messageContent;
        int wordCount;

        public Message(string[] messageContent)
        {
            this.wordCount = messageContent.Length;
            this.messageContent = string.Join(" ", messageContent);


        }

        public override string ToString()
        {
            return wordCount + " " + messageContent;
        }
    }
}