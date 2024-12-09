using System;
using System.Windows;
using System.Windows.Threading;

namespace SharpFocusUI;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var window = new MainWindow();
        this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        window.Show();
    }
    
    void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
        Console.WriteLine(e.Exception);
        string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.Message);
        MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }
}