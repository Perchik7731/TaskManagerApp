using System.Windows;
using TaskManagerApp.Data;
using TaskManagerApp.Models;

namespace TaskManagerApp.Views
{
    public partial class EditProjectWindow : Window
    {
        private DatabaseService _databaseService;
        private Project _project;

        public EditProjectWindow(DatabaseService databaseService, Project project)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _project = project;
            LoadProjectDetails();
        }
        private void LoadProjectDetails()
        {
            TitleTextBox.Text = _project.Title;
            DescriptionTextBox.Text = _project.Description;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _project.Title = TitleTextBox.Text;
            _project.Description = DescriptionTextBox.Text;

            _databaseService.CreateProject(_project); // Обновляем проект
            Close();
        }
    }
}