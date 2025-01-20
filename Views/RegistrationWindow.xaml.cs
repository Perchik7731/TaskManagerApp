using System.Windows;
using System.Windows.Controls;
using TaskManagerApp.Services;
using TaskManagerApp.Data;

namespace TaskManagerApp.Views
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        private DatabaseService _databaseService;
        public RegistrationWindow(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;
            string email = EmailTextBox.Text;

            var authenticationService = new AuthenticationService(_databaseService);
            var registrationResult = authenticationService.RegisterUser(login, password, email);

            if (registrationResult == null)
            {
                MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (registrationResult != null)
            {
                MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }

        }
    }
}
