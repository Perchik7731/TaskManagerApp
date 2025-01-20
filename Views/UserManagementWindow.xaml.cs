using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TaskManagerApp.Data;
using TaskManagerApp.Models;

namespace TaskManagerApp.Views
{
    public partial class UserManagementWindow : Window
    {
        private DatabaseService _databaseService;

        public UserManagementWindow(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;
            LoadUsers();
        }
        public void LoadUsers()
        {
            List<User> users = _databaseService.GetAllUsers();
            UsersDataGrid.ItemsSource = users;
        }
        private void AssignRoleButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is User selectedUser)
            {
                var assignRoleWindow = new AssignRoleWindow(_databaseService, selectedUser);
                assignRoleWindow.ShowDialog();
                LoadUsers();
            }
        }
    }
}