using System.Windows;
using TaskManagerApp.Data;
using TaskManagerApp.Models;

namespace TaskManagerApp.Views
{
    /// <summary>
    /// Логика взаимодействия для AddProjectWindow.xaml
    /// </summary>
    public partial class AddProjectWindow : Window
    {
        private DatabaseService _databaseService;
        public AddProjectWindow(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var project = new Project
            {
                Title = TitleTextBox.Text,
                Description = DescriptionTextBox.Text
            };

            _databaseService.CreateProject(project);
            Close();
        }
    }
}
