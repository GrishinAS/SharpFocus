using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SharpFocusUI;

public partial class RestrictedProgramsSettingsWindow : Window
{
    private readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "restrictedPrograms.txt");

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
        File.WriteAllLines(_filePath, ProgramsList.Items.Cast<string>());
    }
    
    private void LoadItemsFromStorage()
    {
        if (File.Exists(_filePath))
        {
            var items = File.ReadAllLines(_filePath).ToList();
            foreach (string item in items)
            {
                ProgramsList.Items.Add(item);
            }
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}