using System.Windows;
using TaskManagerApp.Data;
using TaskManagerApp.Services;

namespace TaskManagerApp.Views
{
    public partial class ForgotPasswordWindow : Window
    {
        private DatabaseService _databaseService;
        private AuthenticationService _authenticationService;
        public ForgotPasswordWindow(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;
            _authenticationService = new AuthenticationService(_databaseService);
        }
        private void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            if (string.IsNullOrWhiteSpace(login))
            {
                MessageBox.Show("Пожалуйста, введите логин", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = _databaseService.GetUserByLogin(login);
            if (user == null)
            {
                MessageBox.Show("Пользователь с таким логином не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var newPassword = _authenticationService.GenerateRandomPassword();
            user.PasswordHash = _authenticationService.HashPassword(newPassword);
            _databaseService.UpdateUser(user);
            MessageBox.Show($"Новый пароль: {newPassword}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
    }
}