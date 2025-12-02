using CarWashFinancialSystem.Models;
using CarWashFinancialSystem.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CarWashFinancialSystem.Views
{
    public partial class AddUserWindow : Window
    {
        private readonly UserService _userService;
        private User _editingUser;

        public event EventHandler UserAdded;

        public AddUserWindow()
        {
            InitializeComponent();
            _userService = new UserService();
            RoleComboBox.SelectedIndex = 2; // Оператор по умолчанию
            IsActiveCheckBox.Visibility = Visibility.Collapsed; // Скрываем для нового пользователя
        }

        public AddUserWindow(User user) : this()
        {
            _editingUser = user;
            WindowTitleText.Text = "Редактировать пользователя";
            PasswordLabel.Visibility = Visibility.Collapsed;
            PasswordTextBox.Visibility = Visibility.Collapsed;
            IsActiveCheckBox.Visibility = Visibility.Visible;

            // Заполняем поля данными пользователя
            UsernameTextBox.Text = user.Username;
            FullNameTextBox.Text = user.FullName;
            IsActiveCheckBox.IsChecked = user.IsActive;

            // Устанавливаем роль
            foreach (ComboBoxItem item in RoleComboBox.Items)
            {
                if (item.Tag.ToString() == user.Role.ToString())
                {
                    RoleComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text) ||
                string.IsNullOrWhiteSpace(FullNameTextBox.Text) ||
                RoleComboBox.SelectedItem == null)
            {
                MessageBox.Show("Заполните все обязательные поля");
                return;
            }

            // Для нового пользователя проверяем пароль
            if (_editingUser == null && string.IsNullOrWhiteSpace(PasswordTextBox.Text))
            {
                MessageBox.Show("Введите пароль");
                return;
            }

            var selectedRoleItem = RoleComboBox.SelectedItem as ComboBoxItem;
            if (!Enum.TryParse<UserRole>(selectedRoleItem.Tag.ToString(), out UserRole role))
            {
                MessageBox.Show("Выберите корректную роль");
                return;
            }

            bool success;

            if (_editingUser == null)
            {
                // Создание нового пользователя
                success = _userService.RegisterUser(
                    UsernameTextBox.Text.Trim(),
                    PasswordTextBox.Text,
                    FullNameTextBox.Text.Trim(),
                    role
                );
            }
            else
            {
                // Редактирование существующего пользователя
                success = _userService.UpdateUser(
                    _editingUser.Id,
                    UsernameTextBox.Text.Trim(),
                    FullNameTextBox.Text.Trim(),
                    role,
                    IsActiveCheckBox.IsChecked == true
                );
            }

            if (success)
            {
                MessageBox.Show(_editingUser == null ? "Пользователь успешно добавлен" : "Пользователь успешно обновлен");
                UserAdded?.Invoke(this, EventArgs.Empty);
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка при сохранении пользователя. Возможно, логин уже занят.");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}