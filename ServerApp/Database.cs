using System.Collections.Concurrent;

namespace ServerAPP
{
    class Database
    {
        private ConcurrentDictionary<string, string> loginInfo;
        private ConcurrentDictionary<string, string[]> userChats;
        private ConcurrentDictionary<string, Message[]> chats;

        public Database()
        {
            this.loginInfo = new ConcurrentDictionary<string, string>();
            this.userChats = new ConcurrentDictionary<string, string[]>();
            this.chats = new ConcurrentDictionary<string, Message[]>();
        }

        public Task<(string, bool)> registerAccount(string username, string password) {
            bool added = loginInfo.TryAdd(username, password);
            return Task.FromResult(added ? ("User registered", true) : ("Username already taken", false));
        }

    }
    
    class Message
    {
        string messageContent;

        public Message(string messageContent)
        {
            this.messageContent = messageContent;
        }
    }
}