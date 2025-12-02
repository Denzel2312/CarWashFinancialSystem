using CarWashFinancialSystem.Models;
using CarWashFinancialSystem.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace CarWashFinancialSystem.Views.Pages
{
    public partial class UsersPage : Page
    {
        private readonly UserService _userService;
        private ObservableCollection<User> _users;

        public UsersPage()
        {
            InitializeComponent();
            _userService = new UserService();
            LoadUsers();
            UpdateStats();
        }

        private void LoadUsers()
        {
            _users = _userService.GetAllUsers();
            UsersListView.ItemsSource = _users;
        }

        private void UpdateStats()
        {
            var totalUsers = _userService.GetUsersCount();
            var adminsCount = _userService.GetUsersCountByRole(UserRole.Administrator);
            var managersCount = _userService.GetUsersCountByRole(UserRole.Manager);
            var operatorsCount = _userService.GetUsersCountByRole(UserRole.Operator);

            TotalUsersText.Text = totalUsers.ToString();
            AdminsText.Text = adminsCount.ToString();
            ManagersText.Text = managersCount.ToString();
            OperatorsText.Text = operatorsCount.ToString();
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            var addUserWindow = new AddUserWindow();
            addUserWindow.UserAdded += (s, args) =>
            {
                LoadUsers();
                UpdateStats();
            };
            addUserWindow.ShowDialog();
        }

        private void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int userId)
            {
                var user = _userService.GetUserById(userId);
                if (user != null)
                {
                    var editUserWindow = new AddUserWindow(user);
                    editUserWindow.UserAdded += (s, args) =>
                    {
                        LoadUsers();
                        UpdateStats();
                    };
                    editUserWindow.ShowDialog();
                }
            }
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int userId)
            {
                var user = _userService.GetUserById(userId);
                if (user != null)
                {
                    var newPassword = Microsoft.VisualBasic.Interaction.InputBox(
                        $"Введите новый пароль для пользователя {user.Username}:",
                        "Смена пароля",
                        "", -1, -1);

                    if (!string.IsNullOrEmpty(newPassword))
                    {
                        bool success = _userService.ChangePassword(userId, newPassword);
                        if (success)
                        {
                            MessageBox.Show("Пароль успешно изменен");
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при изменении пароля");
                        }
                    }
                }
            }
        }

        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int userId)
            {
                var user = _userService.GetUserById(userId);
                if (user != null)
                {
                    var result = MessageBox.Show(
                        $"Вы уверены, что хотите деактивировать пользователя {user.Username}?",
                        "Подтверждение деактивации",
                        MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        bool success = _userService.DeleteUser(userId);
                        if (success)
                        {
                            MessageBox.Show("Пользователь успешно деактивирован");
                            LoadUsers();
                            UpdateStats();
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при деактивации пользователя");
                        }
                    }
                }
            }
        }
    }
}