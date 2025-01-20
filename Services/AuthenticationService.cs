using System;
using System.Security.Cryptography;
using System.Text;
using TaskManagerApp.Data;
using TaskManagerApp.Models;

namespace TaskManagerApp.Services
{
    public class AuthenticationService
    {
        private readonly DatabaseService _databaseService;

        public AuthenticationService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }
        public User AuthenticateUser(string login, string password)
        {
            var user = _databaseService.GetUserByLogin(login);

            if (user == null)
            {
                return null;
            }

            if (VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }

            return null;
        }
        public User RegisterUser(string login, string password, string email)
        {
            // Проверяем, существует ли пользователь с таким логином
            if (_databaseService.GetUserByLogin(login) != null)
            {
                return null; // Пользователь уже существует
            }

            var user = new User
            {
                Login = login,
                PasswordHash = HashPassword(password),
                Email = email
            };

            var newUserId = _databaseService.CreateUser(user);
            user.Id = newUserId;
            // Проверяем, это первый пользователь?
            if (_databaseService.GetAllUsers().Count == 1)
            {
                // Получаем Id роли "Администратор"
                int adminRoleId = _databaseService.GetUserRoleIdByName("Администратор");
                // Назначаем роль "Администратор" первому пользователю
                _databaseService.AddRoleToUser(user.Id, adminRoleId);
            }

            return user;
        }

        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            string hashedInput = HashPassword(password);
            return string.Equals(hashedInput, hashedPassword, StringComparison.Ordinal);
        }

        public string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] bytes = new byte[10];
                rng.GetBytes(bytes);
                return new string(bytes.Select(x => chars[x % chars.Length]).ToArray());
            }
        }
    }
}
