namespace SharedLib;

public class SharedLibrary
{
    public enum ClientMessageType // message types client use when sending to server
    {
        SendMessage,
        AttemptLogin,
        AttemptRegisterAccount
    }

    public enum ServerMessageType // message types server use when sending to client
    {
        DeliverMessage,
        DeliverMessages,
        DeliverRegisteredAccounts,
        LoginSuccess,
        AccountRegistrationSuccess
    }
}
