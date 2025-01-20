using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TaskManagerApp.Data;
using TaskManagerApp.Models;

namespace TaskManagerApp.Views
{
    public partial class TeamUsersWindow : Window
    {
        private DatabaseService _databaseService;
        private Team _team;
        public TeamUsersWindow(DatabaseService databaseService, Team team)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _team = team;
            TeamNameLabel.Content = _team.Name;
            LoadTeamUsers();
        }
        private void LoadTeamUsers()
        {
            List<User> users = _databaseService.GetTeamUsers(_team.Id);
            UsersListView.ItemsSource = users;
        }

        private void InviteUserButton_Click(object sender, RoutedEventArgs e)
        {
            var inviteUserWindow = new InviteUserWindow(_databaseService, _team);
            inviteUserWindow.ShowDialog();
            LoadTeamUsers();
        }
    }
}