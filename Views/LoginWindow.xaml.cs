using System.Windows;
using System.Windows.Controls;
using TaskManagerApp.Data;
using TaskManagerApp.Services;


namespace TaskManagerApp.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private DatabaseService _databaseService;
        public LoginWindow(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            var authenticationService = new AuthenticationService(_databaseService);
            var user = authenticationService.AuthenticateUser(login, password);

            if (user != null)
            {
                // Успешная аутентификация
                MessageBox.Show("Вход выполнен успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                var mainWindow = new MainWindow();
                MainWindow.CurrentUser = user;
                mainWindow.Show();
                this.Close();
            }
            else
            {
                // Неуспешная аутентификация
                MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registrationWindow = new RegistrationWindow(_databaseService);
            registrationWindow.ShowDialog();
        }

        private void ForgotPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var forgotPasswordWindow = new ForgotPasswordWindow(_databaseService);
            forgotPasswordWindow.ShowDialog();
        }
    }
}
