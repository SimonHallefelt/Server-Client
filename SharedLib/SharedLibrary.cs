namespace SharedLib;

public class SharedLibrary
{
    public enum ClientMessageType // message types client use when sending to server
    {
        SendMessage,
        AttemptLogin,
        AttemptRegisterAccount,
        RequestChatLogFor,
        RequestUpdateFromServer
    }

    public enum ServerMessageType // message types server use when sending to client
    {
        DeliverMessage,
        DeliverMessages,
        DeliverRegisteredAccounts,
        LoginSuccess,
        AccountRegistrationSuccess
    }

    public class Message
    {
        private string messageContent;
        private int wordCount;
        private string sender;
        private DateTime dateTime;

        public Message(string sender, DateTime dateTime, string[] messageContent)
        {
            this.wordCount = messageContent.Length;
            this.messageContent = string.Join(" ", messageContent);
            this.sender = sender;
            this.dateTime = dateTime;
        }

        public string GetMessageContent()
        {
            return messageContent;
        }

        public override string ToString()
        {
            return $"{sender} {dateTime} {wordCount} {messageContent}";
        }

    }
}
