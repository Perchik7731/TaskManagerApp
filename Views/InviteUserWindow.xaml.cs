using System.Collections.Generic;
using System.Windows;
using TaskManagerApp.Data;
using TaskManagerApp.Models;

namespace TaskManagerApp.Views
{
    public partial class InviteUserWindow : Window
    {
        private DatabaseService _databaseService;
        private Team _team;
        public InviteUserWindow(DatabaseService databaseService, Team team)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _team = team;
            LoadUsers();
        }
        private void LoadUsers()
        {
            List<User> users = _databaseService.GetAllUsers();
            UsersComboBox.ItemsSource = users;
        }

        private void InviteButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersComboBox.SelectedItem is User selectedUser)
            {
                _databaseService.AddUserToTeam(selectedUser.Id, _team.Id);
                MessageBox.Show($"Пользователь {selectedUser.Login} добавлен в команду {_team.Name}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            else
            {
                MessageBox.Show("Выберите пользователя для добавления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}