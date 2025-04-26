using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace ClientApp {
    public class App : Application {
        public override void Initialize() {
            AvaloniaXamlLoader.Load(this); // Loads App.axaml
        }

        public override void OnFrameworkInitializationCompleted() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                desktop.MainWindow = new MainWindow(); // Sets MainWindow
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}