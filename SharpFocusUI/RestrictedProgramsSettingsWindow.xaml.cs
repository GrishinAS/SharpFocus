using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;

namespace SharpFocusUI;

public partial class RestrictedProgramsSettingsWindow : Window
{

    public RestrictedProgramsSettingsWindow()
    {
        InitializeComponent();
        LoadItemsFromStorage();
    }

    private void AddProgram_Click(object sender, RoutedEventArgs e)
    {
        ProgramsList.Items.Add(NewItemTextBox.Text);
    }

    private void RemoveProgram_Click(object sender, RoutedEventArgs e)
    {
        var selectedItem = ProgramsList.SelectedItem;
        if (selectedItem != null)
        {
            ProgramsList.Items.Remove(selectedItem.ToString());
        }
    }

    private void SaveRestrictedProgramsSettings_Click(object sender, RoutedEventArgs e)
    {
        SaveItemsToStorage();
        Close();
    }
    
    private void SaveItemsToStorage()
    {
        AppSettings appSettings = MainWindow.LoadSettings();
        appSettings.RestrictedPrograms = ProgramsList.Items.Cast<string>().ToList();
        
        string json = JsonSerializer.Serialize(appSettings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(MainWindow.SettingsFilePath, json);
    }
    
    private void LoadItemsFromStorage()
    {
        AppSettings appSettings = MainWindow.LoadSettings();
        foreach (string item in appSettings.RestrictedPrograms)
        {
            ProgramsList.Items.Add(item);
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}