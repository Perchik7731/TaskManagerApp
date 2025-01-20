using System.Windows;
using TaskManagerApp.Data;
using TaskManagerApp.Models;

namespace TaskManagerApp.Views
{
    public partial class AssignRoleWindow : Window
    {
        private DatabaseService _databaseService;
        private User _user;

        public AssignRoleWindow(DatabaseService databaseService, User user)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _user = user;
            LoadRoles();
            UserLoginTextBlock.Text = _user.Login;
        }
        private void LoadRoles()
        {
            List<string> roles = _databaseService.GetAllUserRoles();
            RolesComboBox.ItemsSource = roles;
        }

        private void AssignButton_Click(object sender, RoutedEventArgs e)
        {
            if (RolesComboBox.SelectedItem is string selectedRole)
            {
                var roleId = _databaseService.GetUserRoleIdByName(selectedRole);
                if (roleId == 0) 
                {
                    var newRoleId = _databaseService.CreateUserRole(selectedRole);
                    _databaseService.AddRoleToUser(_user.Id, newRoleId);
                }
                else
                {
                    _databaseService.AddRoleToUser(_user.Id, roleId);
                }

                MessageBox.Show($"Роль {selectedRole} успешно назначена пользователю {_user.Login}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                if (Owner is UserManagementWindow userManagementWindow)
                {
                    userManagementWindow.LoadUsers();
                }
                Close();
            }
            else
            {
                MessageBox.Show("Выберите роль для пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}