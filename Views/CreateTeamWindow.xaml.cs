using System.Windows;
using TaskManagerApp.Data;
using TaskManagerApp.Models;

namespace TaskManagerApp.Views
{
    public partial class CreateTeamWindow : Window
    {
        private DatabaseService _databaseService;
        public CreateTeamWindow(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var team = new Team
            {
                Name = NameTextBox.Text,
                Description = DescriptionTextBox.Text
            };
            _databaseService.CreateTeam(team);
            Close();
        }
    }
}