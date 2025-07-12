using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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
    
        private async Task ReceiveMessages() {
            while (true) {
                (uint versionNumber, string message) = await api.GetMessage();
                if (message == null) continue;
                await mainWindow.AddNewMessage(message);
            }
        }
    }
}
    