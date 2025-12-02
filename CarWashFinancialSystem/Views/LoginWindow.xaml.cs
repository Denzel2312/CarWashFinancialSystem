using CarWashFinancialSystem.Services;
using CarWashFinancialSystem.Views;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace CarWashFinancialSystem.Views
{
    public partial class LoginWindow : Window
    {
        private readonly UserService _userService;

        public LoginWindow()
        {
            InitializeComponent();
            _userService = new UserService();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                App.Logger.LogWarning("Предоставлены пустые учетные данные", user: username); MessageBox.Show("Введите логин и пароль");
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            var user = _userService.Authenticate(username, password);
            if (user != null)
            {
                App.Logger.LogUserAction("Успешный вход", user.Username); var mainWindow = new MainWindow(user);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                App.Logger.LogUserAction("Неудачная попытка входа", username);
                MessageBox.Show("Неверный логин или пароль");
            }

        }

    }
}