using System.Windows;
using CarWashFinancialSystem.Services;
using CarWashFinancialSystem.Views;

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
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            var user = _userService.Authenticate(username, password);
            if (user != null)
            {
                var mainWindow = new MainWindow(user);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль");
            }
        }
    }
}