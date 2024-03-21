using System.Windows;

namespace SharpFocusUI;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var window = new MainWindow();
        window.Show();
    }
}