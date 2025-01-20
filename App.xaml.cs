using System;
using System.Windows;
using TaskManagerApp.Data;
using TaskManagerApp.Views;
using System.IO;
using System.Xml.Linq;

namespace TaskManagerApp
{
    public partial class App : Application
    {
        public static event EventHandler ThemeChanged;
        private const string SettingsFileName = "settings.xml";
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LoadTheme();

            var databaseService = new DatabaseService();
            var loginWindow = new LoginWindow(databaseService);
            loginWindow.Show();
        }
        public static void OnThemeChanged(object sender, EventArgs e)
        {
            ThemeChanged?.Invoke(sender, e);
        }

        private void LoadTheme()
        {
            if (!File.Exists(SettingsFileName))
            {
                ChangeTheme("Светлая");
                return;
            }
            try
            {
                XDocument doc = XDocument.Load(SettingsFileName);
                XElement root = doc.Root;
                if (root != null)
                {
                    var themeElement = root.Element("Theme");
                    if (themeElement != null)
                    {
                        string theme = themeElement.Value;
                        ChangeTheme(theme);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки настроек: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void ChangeTheme(string theme)
        {
            if (theme == "Светлая")
            {
                Resources.MergedDictionaries.Clear();
                Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Themes/LightTheme.xaml", UriKind.Relative) });
            }
            else if (theme == "Темная")
            {
                Resources.MergedDictionaries.Clear();
                Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Themes/DarkTheme.xaml", UriKind.Relative) });
            }
        }
        public void SaveTheme(string theme)
        {
            try
            {
                XDocument doc = new XDocument(new XElement("Settings", new XElement("Theme", theme)));
                doc.Save(SettingsFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения настроек: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}