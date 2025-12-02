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
            switch (user.Role)
            {
                case UserRole.Operator:
                    MainFrame.Content = new TransactionsPage(); // Оператор сразу видит транзакции
                    break;
                case UserRole.Manager:
                case UserRole.Administrator:
                    MainFrame.Content = new DashboardPage(); // Менеджер и админ видят аналитику
                    break;
            }
        }

        private void ShowDashboard()
        {
            MainFrame.Content = new DashboardPage();
        }

        private void DashboardBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.Role != UserRole.Administrator && CurrentUser.Role != UserRole.Manager)
            {
                MessageBox.Show("Доступ запрещен. Только менеджеры и администраторы могут просматривать аналитику.");
                return;
            }
            MainFrame.Content = new DashboardPage();
        }

        private void ServicesBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.Role != UserRole.Administrator && CurrentUser.Role != UserRole.Manager)
            {
                MessageBox.Show("Доступ запрещен. Только менеджеры и администраторы могут просматривать аналитику.");
                return;
            }
            MainFrame.Content = new ServicesPage();
        }

        private void TransactionsBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new TransactionsPage();
        }

        private void ExpensesBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.Role != UserRole.Administrator && CurrentUser.Role != UserRole.Manager)
            {
                MessageBox.Show("Доступ запрещен. Только менеджеры и администраторы могут просматривать аналитику.");
                return;
            }
            MainFrame.Content = new ExpensesPage();
        }

        private void ReportsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.Role != UserRole.Administrator && CurrentUser.Role != UserRole.Manager)
            {
                MessageBox.Show("Доступ запрещен. Только менеджеры и администраторы могут просматривать аналитику.");
                return;
            }
            MainFrame.Content = new ReportsPage();
        }

        private void UsersBtn_Click(object sender, RoutedEventArgs e)
        {
            // Временно показываем страницу пользователей всем
            // Позже добавим проверку ролей
            if (CurrentUser.Role != UserRole.Administrator)
            {
                MessageBox.Show("Доступ запрещен. Только администраторы могут управлять пользователями.");
                return;
            }
            MainFrame.Content = new UsersPage();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
        public void NavigateToReports()
        {
            ReportsBtn_Click(null, null);
        }
    }
}