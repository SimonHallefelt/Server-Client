using System.Collections.Concurrent;
using Message = SharedLib.SharedLibrary.Message;

namespace ServerAPP
{
    class Database
    {
        private ConcurrentDictionary<string, string> loginInfo;
        private ConcurrentDictionary<string, LinkedList<Message>> chats;

        public Database()
        {
            this.loginInfo = new ConcurrentDictionary<string, string>();
            this.chats = new ConcurrentDictionary<string, LinkedList<Message>>();
        }

        public Task<(string, bool)> addMessage(string sender, string receiver, string[] messageContent)
        {
            try
            {
                DateTime time = System.DateTime.Now;
                Message message = new Message(sender, time, messageContent);
                string key = makeKey(sender, receiver);
                if (chats.TryGetValue(key, out var list))
                {
                    list.AddLast(message);
                    Console.WriteLine($"added a new message to {sender} and {receiver}");
                    return Task.FromResult((message.ToString(), true));
                }
                else
                {
                    chats.TryAdd(key, new LinkedList<Message>(new[] { message }));
                    Console.WriteLine($"made a new message list for {sender} and {receiver}");
                    return Task.FromResult((message.ToString(), true));
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
            string key = makeKey(user1, user2);
            if (chats.TryGetValue(key, out var list))
            {
                string messagesString = "";
                foreach (Message message in list)
                {
                    messagesString += message.ToString() + " ";
                }
                return Task.FromResult((messagesString, true));
            }
            return Task.FromResult(("", false));
        }

        private string makeKey(string user1, string user2)
        {
            return (string.Compare(user1, user2) < 0 ? user1 : user2) + " " + (string.Compare(user1, user2) < 0 ? user2 : user1);
        }
    }
}