using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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
        private RestrictedProgramsSettingsWindow? _restrictedProgramsSettingsWindow;
        private BlockingModeSettingsWindow? _blockingModeSettingsWindow;
        public static readonly string SettingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "focus_settings.cfg");

        public MainWindow()
        {
            InitializeComponent();
            AppSettings loadSettings = LoadSettings();
            LeetCodeUsername.Text = loadSettings.LeetCodeUsername;
            _processMonitor = new ProcessMonitor(ShowMsg, new LeetcodeClient());
        }


        private void StartFocus_Click(object sender, RoutedEventArgs e)
        {
            _focusProcessTokenSource = new CancellationTokenSource();
            Task.Run(() => _processMonitor.Start(_focusProcessTokenSource), _focusProcessTokenSource.Token);
            
            Start.IsEnabled = false;
            Stop.IsEnabled = true;
        }
        
        private void StopFocus_Click(object sender, RoutedEventArgs e)
        {
            _focusProcessTokenSource?.Cancel();
            Stop.IsEnabled = false;
            Start.IsEnabled = true;
        }

        private void OpenBlockingModeSettings_Click(object sender, RoutedEventArgs e)
        {
            if (_blockingModeSettingsWindow == null)
            {
                _blockingModeSettingsWindow = new BlockingModeSettingsWindow();
                _blockingModeSettingsWindow.Closed += BlockingModeSettingsWindow_Closed;
                _blockingModeSettingsWindow.Show();
            }
            else
            {
                if (_blockingModeSettingsWindow.Visibility == Visibility.Hidden)
                {
                    _blockingModeSettingsWindow.Visibility = Visibility.Visible;
                }
                else
                {
                    _blockingModeSettingsWindow.Focus();
                }
            }
        }

        private void OpenRestrictedProgramsSettings_Click(object sender, RoutedEventArgs e)
        {
            if (_restrictedProgramsSettingsWindow == null)
            {
                _restrictedProgramsSettingsWindow = new RestrictedProgramsSettingsWindow();
                _restrictedProgramsSettingsWindow.Closed += RestrictedProgramsSettingsWindow_Closed;
                _restrictedProgramsSettingsWindow.Show();
            }
            else
            {
                if (_restrictedProgramsSettingsWindow.Visibility == Visibility.Hidden)
                {
                    _restrictedProgramsSettingsWindow.Visibility = Visibility.Visible;
                }
                else
                {
                    _restrictedProgramsSettingsWindow.Focus();
                }
            }
        }
        
        private void RestrictedProgramsSettingsWindow_Closed(object? sender, EventArgs e)
        {
            _restrictedProgramsSettingsWindow = null;
        }
        
        private void BlockingModeSettingsWindow_Closed(object? sender, EventArgs e)
        {
            _blockingModeSettingsWindow = null;
        }
        
        private static void ShowMsg(string message)
        {
            MessageBox.Show(null, message, MessageBoxButton.OK);
        }


        private void SaveUsername_Click(object sender, RoutedEventArgs e)
        {
            AppSettings appSettings = LoadSettings();
            appSettings.LeetCodeUsername = LeetCodeUsername.Text;
            string json = JsonSerializer.Serialize(appSettings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsFilePath, json);
        }
        
        public static AppSettings LoadSettings()
        {
            if (File.Exists(SettingsFilePath))
            {
                string json = File.ReadAllText(SettingsFilePath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? throw new InvalidOperationException("Failed to load settings");
            }
            return new AppSettings();
        }
    }
}