using CarWashFinancialSystem.Models;
using CarWashFinancialSystem.Views.Pages;
using System.Windows;

namespace CarWashFinancialSystem.Views
{
    public partial class MainWindow : Window
    {
        public User CurrentUser { get; set; }

        public MainWindow(User user)
        {
            InitializeComponent();
            CurrentUser = user;
            UserNameText.Text = user.FullName + $" ({user.Role})";

            // Показываем дашборд по умолчанию
            ShowDashboard();
        }

        private void ShowDashboard()
        {
            MainFrame.Content = new DashboardPage();
        }

        private void DashboardBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DashboardPage();
        }

        private void ServicesBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ServicesPage();
        }

        private void TransactionsBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new TransactionsPage();
        }

        private void ExpensesBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ExpensesPage();
        }

        private void ReportsBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ReportsPage();
        }

        private void UsersBtn_Click(object sender, RoutedEventArgs e)
        {
            // Временно показываем страницу пользователей всем
            // Позже добавим проверку ролей
            MainFrame.Content = new UsersPage();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}