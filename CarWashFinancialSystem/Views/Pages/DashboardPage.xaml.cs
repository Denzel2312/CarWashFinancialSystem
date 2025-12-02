using CarWashFinancialSystem.Services;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CarWashFinancialSystem.Views.Pages
{
    public partial class DashboardPage : Page
    {
        private readonly ChartService _chartService;
        private readonly TransactionService _transactionService;
        private readonly ExpenseService _expenseService;

        // Data for charts
        public SeriesCollection RevenueSeries { get; set; }
        public SeriesCollection ServiceSeries { get; set; }
        public SeriesCollection RevenueVsExpensesSeries { get; set; }
        public SeriesCollection PaymentMethodsSeries { get; set; }

        public string[] RevenueLabels { get; set; }
        public string[] RevenueVsExpensesLabels { get; set; }

        public Func<double, string> RevenueFormatter { get; set; }
        public Func<double, string> AmountFormatter { get; set; }

        public DashboardPage()
        {
            InitializeComponent();
            _chartService = new ChartService();
            _transactionService = new TransactionService();
            _expenseService = new ExpenseService();

            DataContext = this;
            InitializeCharts();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            UpdateCharts();
        }

        private void InitializeCharts()
        {
            // Инициализация коллекций
            RevenueSeries = new SeriesCollection();
            ServiceSeries = new SeriesCollection();
            RevenueVsExpensesSeries = new SeriesCollection();
            PaymentMethodsSeries = new SeriesCollection();

            // Форматтеры для осей
            RevenueFormatter = value => value.ToString("N0") + " руб";
            AmountFormatter = value => value.ToString("N0") + " руб";
        }

        private void LoadData()
        {
            // Обновляем KPI карточки
            var todayRevenue = _transactionService.GetTodayRevenue();
            var todayTransactions = _transactionService.GetTodayTransactionsCount();
            var avgCheck = todayTransactions > 0 ? todayRevenue / todayTransactions : 0;

            // Расчет прибыли за месяц
            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var monthlyRevenue = _transactionService.GetAllTransactions()
                .Where(t => t.TransactionDate >= firstDayOfMonth)
                .Sum(t => t.Amount);
            var monthlyExpenses = _expenseService.GetAllExpenses()
                .Where(e => e.ExpenseDate >= firstDayOfMonth)
                .Sum(e => e.Amount);
            var monthlyProfit = monthlyRevenue - monthlyExpenses;

            TodayRevenueText.Text = $"{todayRevenue} руб";
            TodayTransactionsText.Text = todayTransactions.ToString();
            AvgCheckText.Text = $"{avgCheck:F0} руб";
            MonthlyProfitText.Text = $"{monthlyProfit} руб";

            // Простые изменения (можно заменить на реальные расчеты)
            TodayRevenueChange.Text = "+12%";
            TodayTransactionsChange.Text = "+5%";
            AvgCheckChange.Text = "+3%";
            MonthlyProfitChange.Text = "+8%";
        }

        private void UpdateCharts()
        {
            UpdateRevenueChart();
            UpdateServicesPieChart();
            UpdateRevenueVsExpensesChart();
            UpdatePaymentMethodsChart();
        }

        private void UpdateRevenueChart()
        {
            var revenueData = _chartService.GetRevenueLast7Days();

            RevenueLabels = revenueData.Keys.Select(d => d.ToString("dd.MM")).ToArray();

            RevenueSeries.Clear();
            RevenueSeries.Add(new LineSeries
            {
                Title = "Выручка",
                Values = new ChartValues<decimal>(revenueData.Values),
                Stroke = new SolidColorBrush(Color.FromRgb(76, 175, 80)),
                Fill = Brushes.Transparent,
                PointGeometry = DefaultGeometries.Circle,
                PointForeground = new SolidColorBrush(Color.FromRgb(76, 175, 80)),
                StrokeThickness = 3
            });
        }

        private void UpdateServicesPieChart()
        {
            var serviceData = _chartService.GetServiceDistributionCurrentMonth();

            ServiceSeries.Clear();

            var colors = new[]
            {
                Color.FromRgb(126, 87, 194),  // Primary
                Color.FromRgb(255, 152, 0),   // Orange
                Color.FromRgb(33, 150, 243),  // Blue
                Color.FromRgb(76, 175, 80),   // Green
                Color.FromRgb(156, 39, 176),  // Purple
                Color.FromRgb(244, 67, 54)    // Red
            };

            int colorIndex = 0;
            foreach (var item in serviceData)
            {
                ServiceSeries.Add(new PieSeries
                {
                    Title = item.Key,
                    Values = new ChartValues<decimal> { item.Value },
                    DataLabels = true,
                    LabelPoint = point => $"{point.Y:N0} руб",
                    Fill = new SolidColorBrush(colors[colorIndex % colors.Length])
                });
                colorIndex++;
            }
        }

        private void UpdateRevenueVsExpensesChart()
        {
            var (revenueData, expensesData) = _chartService.GetRevenueVsExpensesLast30Days();

            // Берем только каждую 5-ю дату для labels чтобы не перегружать
            RevenueVsExpensesLabels = revenueData.Keys
                .Where((date, index) => index % 5 == 0)
                .Select(d => d.ToString("dd.MM"))
                .ToArray();

            RevenueVsExpensesSeries.Clear();
            RevenueVsExpensesSeries.Add(new ColumnSeries
            {
                Title = "Доходы",
                Values = new ChartValues<decimal>(revenueData.Values),
                Fill = new SolidColorBrush(Color.FromRgb(76, 175, 80))
            });

            RevenueVsExpensesSeries.Add(new ColumnSeries
            {
                Title = "Расходы",
                Values = new ChartValues<decimal>(expensesData.Values),
                Fill = new SolidColorBrush(Color.FromRgb(244, 67, 54))
            });
        }

        private void UpdatePaymentMethodsChart()
        {
            var paymentData = _chartService.GetPaymentMethodStats();

            PaymentMethodsSeries.Clear();

            var colors = new[]
            {
                Color.FromRgb(126, 87, 194),  // Primary
                Color.FromRgb(33, 150, 243),  // Blue
                Color.FromRgb(255, 152, 0),   // Orange
            };

            int colorIndex = 0;
            foreach (var item in paymentData)
            {
                PaymentMethodsSeries.Add(new PieSeries
                {
                    Title = item.Key,
                    Values = new ChartValues<int> { item.Value },
                    DataLabels = true,
                    LabelPoint = point => $"{point.Y} транзакций",
                    Fill = new SolidColorBrush(colors[colorIndex % colors.Length])
                });
                colorIndex++;
            }
        }

        private void AddTransaction_Click(object sender, RoutedEventArgs e)
        {
            var addTransactionWindow = new AddTransactionWindow();
            addTransactionWindow.TransactionAdded += (s, args) =>
            {
                LoadData();
                UpdateCharts();
            };
            addTransactionWindow.ShowDialog();
        }

        private void DetailedReport_Click(object sender, RoutedEventArgs e)
        {
            // Переход к детальным отчетам
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.NavigateToReports();
        }

        private void RefreshData_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            UpdateCharts();
            MessageBox.Show("Данные обновлены");
        }

    }
}