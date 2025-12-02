using CarWashFinancialSystem.Services;
using System.Windows;
using System.Windows.Controls;

namespace CarWashFinancialSystem.Views.Pages
{
    public partial class TransactionsPage : Page
    {
        private readonly TransactionService _transactionService;

        public TransactionsPage()
        {
            InitializeComponent();
            _transactionService = new TransactionService();
            LoadTransactions();
            UpdateStats();
        }

        private void LoadTransactions()
        {
            var transactions = _transactionService.GetAllTransactions();
            TransactionsListView.ItemsSource = transactions;
        }

        private void UpdateStats()
        {
            var todayRevenue = _transactionService.GetTodayRevenue();
            var todayCount = _transactionService.GetTodayTransactionsCount();

            TodayRevenueText.Text = $"{todayRevenue} руб";
            TodayTransactionsText.Text = todayCount.ToString();
        }

        private void AddTransactionButton_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно добавления транзакции
            var addTransactionWindow = new AddTransactionWindow();
            addTransactionWindow.TransactionAdded += (s, args) =>
            {
                App.Logger.LogInfo("Новая транзакция добавлена ​​через диалог", component: "Система транзакций");
                LoadTransactions();
                UpdateStats();
            };
            addTransactionWindow.ShowDialog();
        }
    }
}