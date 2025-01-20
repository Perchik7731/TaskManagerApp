using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TaskManagerApp.Data;
using TaskManagerApp.Models;
using TaskManagerApp.Services;

namespace TaskManagerApp.Views
{
    public partial class UserProfileWindow : Window
    {
        private DatabaseService _databaseService;
        private User _currentUser;
        private AuthenticationService _authenticationService;
        public UserProfileWindow()
        {
            InitializeComponent();
        }

        public UserProfileWindow(DatabaseService databaseService, User currentUser)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _currentUser = currentUser;
            _authenticationService = new AuthenticationService(_databaseService);
            LoadUserData();
            LoadUserTasks();
            LoadUserRoles();
        }
        private void LoadUserData()
        {
            LoginTextBlock.Text = _currentUser.Login;
            EmailTextBlock.Text = _currentUser.Email;
        }
        private void LoadUserTasks()
        {
            List<Models.Task> tasks = _databaseService.GetTasksByUserId(_currentUser.Id);
            UserTasksListView.ItemsSource = tasks;
        }
        private void LoadUserRoles()
        {
            List<string> roles = _databaseService.GetUserRoles(_currentUser.Id);
            if (roles != null && roles.Any())
            {
                RolesTextBlock.Text = string.Join(", ", roles);
            }
            else
            {
                RolesTextBlock.Text = "Нет ролей";
            }
        }
        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            string currentPassword = CurrentPasswordBox.Password;
            string newPassword = NewPasswordBox.Password;
            if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("Пожалуйста, введите текущий и новый пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!_authenticationService.VerifyPassword(currentPassword, _currentUser.PasswordHash))
            {
                MessageBox.Show("Текущий пароль введен неверно", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            _currentUser.PasswordHash = _authenticationService.HashPassword(newPassword);
            _databaseService.UpdateUser(_currentUser);
            MessageBox.Show("Пароль успешно изменен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            CurrentPasswordBox.Clear();
            NewPasswordBox.Clear();
        }
    }
}