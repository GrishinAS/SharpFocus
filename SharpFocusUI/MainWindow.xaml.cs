using System;
using System.Threading;
using System.Windows;
using FocusProcess;

namespace SharpFocusUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ProcessMonitor _processMonitor;
        private CancellationTokenSource? _focusProcessTokenSource;
        private RestrictedProgramsSettingsWindow? restrictedProgramsSettingsWindow;
        private BlockingModeSettingsWindow? blockingModeSettingsWindow;

        public MainWindow()
        {
            InitializeComponent();
            _processMonitor = new ProcessMonitor(ShowProcessKilledMsg, new LeetcodeClient());
        }


        private void StartFocus_Click(object sender, RoutedEventArgs e)
        {
            _focusProcessTokenSource = new CancellationTokenSource();
            _processMonitor.Start(_focusProcessTokenSource.Token);
            Start.IsEnabled = false;
            Stop.IsEnabled = true;
        }
        
        private void StopFocus_Click(object sender, RoutedEventArgs e)
        {
            _focusProcessTokenSource?.Cancel();
            Stop.IsEnabled = false;
            Start.IsEnabled = true;
        }
        
        private void SkipProtection_Click(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void OpenBlockingModeSettings_Click(object sender, RoutedEventArgs e)
        {
            if (blockingModeSettingsWindow == null)
            {
                blockingModeSettingsWindow = new BlockingModeSettingsWindow();
                blockingModeSettingsWindow.Closed += BlockingModeSettingsWindow_Closed;
                blockingModeSettingsWindow.Show();
            }
            else
            {
                if (blockingModeSettingsWindow.Visibility == Visibility.Hidden)
                {
                    blockingModeSettingsWindow.Visibility = Visibility.Visible;
                }
                else
                {
                    blockingModeSettingsWindow.Focus();
                }
            }
        }

        private void OpenRestrictedProgramsSettings_Click(object sender, RoutedEventArgs e)
        {
            if (restrictedProgramsSettingsWindow == null)
            {
                restrictedProgramsSettingsWindow = new RestrictedProgramsSettingsWindow();
                restrictedProgramsSettingsWindow.Closed += RestrictedProgramsSettingsWindow_Closed;
                restrictedProgramsSettingsWindow.Show();
            }
            else
            {
                if (restrictedProgramsSettingsWindow.Visibility == Visibility.Hidden)
                {
                    restrictedProgramsSettingsWindow.Visibility = Visibility.Visible;
                }
                else
                {
                    restrictedProgramsSettingsWindow.Focus();
                }
            }
        }
        
        private void RestrictedProgramsSettingsWindow_Closed(object? sender, EventArgs e)
        {
            restrictedProgramsSettingsWindow = null;
        }
        
        private void BlockingModeSettingsWindow_Closed(object? sender, EventArgs e)
        {
            blockingModeSettingsWindow = null;
        }
        
        private static void ShowProcessKilledMsg(string processName)
        {
            MessageBox.Show(null, $"Restricted process '{processName}' was killed. Solve your daily Leetcode task to run it", MessageBoxButton.OK);
        }


    }
}