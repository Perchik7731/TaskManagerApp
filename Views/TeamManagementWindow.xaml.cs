using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TaskManagerApp.Data;
using TaskManagerApp.Models;

namespace TaskManagerApp.Views
{
    public partial class TeamManagementWindow : Window
    {
        private DatabaseService _databaseService;
        public TeamManagementWindow(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;
            LoadTeams();
        }
        private void LoadTeams()
        {
            List<Team> teams = _databaseService.GetAllTeams();
            TeamsDataGrid.ItemsSource = teams;
        }
        private void CreateTeamButton_Click(object sender, RoutedEventArgs e)
        {
            var createTeamWindow = new CreateTeamWindow(_databaseService);
            createTeamWindow.ShowDialog();
            LoadTeams();
        }
        private void ViewTeamUsersButton_Click(object sender, RoutedEventArgs e)
        {
            if (TeamsDataGrid.SelectedItem is Team selectedTeam)
            {
                var teamUsersWindow = new TeamUsersWindow(_databaseService, selectedTeam);
                teamUsersWindow.ShowDialog();
            }
        }
        private void DeleteTeamButton_Click(object sender, RoutedEventArgs e)
        {
            if (TeamsDataGrid.SelectedItem is Team selectedTeam)
            {
                _databaseService.DeleteTeam(selectedTeam.Id);
                LoadTeams();
            }
        }
    }
}