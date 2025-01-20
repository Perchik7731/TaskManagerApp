using System;
using System.Windows;
using System.Windows.Controls;
using TaskManagerApp.Models;

namespace TaskManagerApp.Views
{
    public partial class SettingsWindow : Window
    {
        private string _selectedTheme;
        public SettingsWindow()
        {
            InitializeComponent();
            ThemeComboBox.Items.Add(new ComboBoxItem() { Content = "Светлая" });
            ThemeComboBox.Items.Add(new ComboBoxItem() { Content = "Темная" });
        }
        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (ThemeComboBox.SelectedItem is ComboBoxItem selectedTheme)
            {
                _selectedTheme = selectedTheme.Content.ToString();
                var app = (App)Application.Current;
                app.ChangeTheme(_selectedTheme);
                app.SaveTheme(_selectedTheme);
                Close();
            }
            else
            {
                MessageBox.Show("Выберите тему", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}