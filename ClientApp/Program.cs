using Avalonia;
using Avalonia.ReactiveUI;
using System;
using System.Runtime.InteropServices;

namespace ClientApp {
    internal class Program {

        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();

        [STAThread]
        public static void Main(string[] args) {
            AllocConsole();
            Console.WriteLine("Started A Client");
            try {
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            } catch (Exception ex) {
                Console.WriteLine($"Unhandled exception: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        public static AppBuilder BuildAvaloniaApp() {
            return AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .LogToTrace()
                         .UseReactiveUI();
        }
    }
}