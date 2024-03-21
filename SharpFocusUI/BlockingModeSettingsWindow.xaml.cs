using System.Windows;

namespace SharpFocusUI;

public partial class BlockingModeSettingsWindow : Window
{
    private readonly bool _savedIsBlockingModeEnabled;
    private readonly object _savedNotificationInterval;
    public BlockingModeSettingsWindow()
    {
        InitializeComponent();
        _savedIsBlockingModeEnabled = BlockingModeCheckbox.IsChecked ?? false;
        _savedNotificationInterval = NotificationIntervalDropdown.SelectedItem;
    }


    private void OK_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Cancel_OnClick(object sender, RoutedEventArgs e)
    {
        BlockingModeCheckbox.IsChecked = _savedIsBlockingModeEnabled;
        NotificationIntervalDropdown.SelectedItem = _savedNotificationInterval;
        Close();
    }
}