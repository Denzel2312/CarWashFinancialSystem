using CarWashFinancialSystem.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CarWashFinancialSystem.Views.Pages
{
    public partial class ReportsPage : Page
    {
        private readonly ReportService _reportService;
        private readonly ExcelExportService _excelExportService;

        public ReportsPage()
        {
            InitializeComponent();
            _reportService = new ReportService();
            _excelExportService = new ExcelExportService();
            // Установка дат по умолчанию (текущий месяц)
            StartDatePicker.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            EndDatePicker.SelectedDate = DateTime.Now;

            // Генерация отчета при загрузке
            GenerateReport();
        }

        private void GenerateReport()
        {
            if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
                return;

            var startDate = StartDatePicker.SelectedDate.Value;
            var endDate = EndDatePicker.SelectedDate.Value.AddDays(1);

            // Финансовая сводка
            dynamic summary = _reportService.GetFinancialSummary(startDate, endDate); // ← ИСПРАВЛЕНО: используем dynamic
            RevenueText.Text = $"{summary.TotalRevenue} руб";
            ExpensesText.Text = $"{summary.TotalExpenses} руб";
            ProfitText.Text = $"{summary.Profit} руб";
            TransactionsText.Text = summary.TransactionCount.ToString();
            AvgCheckText.Text = $"{summary.AverageCheck:F2} руб";

            // Популярные услуги
            var popularServices = _reportService.GetPopularServices(startDate, endDate);
            PopularServicesListView.ItemsSource = popularServices;

            // Отчет по дням
            var dailyReport = _reportService.GetDailyReport(startDate, endDate.AddDays(-1));
            DailyReportListView.ItemsSource = dailyReport;
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            GenerateReport();
        }

        private void ExportReport_Click(object sender, RoutedEventArgs e)
        {
            ExportReport("full"); // Полный отчет по умолчанию
        }

        // Быстрый выбор периодов
        private void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            StartDatePicker.SelectedDate = DateTime.Today;
            EndDatePicker.SelectedDate = DateTime.Today;
            GenerateReport();
        }

        private void WeekButton_Click(object sender, RoutedEventArgs e)
        {
            StartDatePicker.SelectedDate = DateTime.Today.AddDays(-7);
            EndDatePicker.SelectedDate = DateTime.Today;
            GenerateReport();
        }

        private void MonthButton_Click(object sender, RoutedEventArgs e)
        {
            StartDatePicker.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            EndDatePicker.SelectedDate = DateTime.Today;
            GenerateReport();
        }

        private void QuarterButton_Click(object sender, RoutedEventArgs e)
        {
            var currentQuarter = (DateTime.Now.Month - 1) / 3 + 1;
            var startMonth = (currentQuarter - 1) * 3 + 1;
            StartDatePicker.SelectedDate = new DateTime(DateTime.Now.Year, startMonth, 1);
            EndDatePicker.SelectedDate = DateTime.Today;
            GenerateReport();
        }

        private void ExportReport(string reportType)
        {
            if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите период для экспорта");
                return;
            }

            try
            {
                var startDate = StartDatePicker.SelectedDate.Value;
                var endDate = EndDatePicker.SelectedDate.Value;

                // Показываем прогресс
                var progressWindow = new Window
                {
                    Title = "Экспорт в Excel",
                    Width = 300,
                    Height = 100,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Content = new StackPanel
                    {
                        Children =
                {
                    new ProgressBar { IsIndeterminate = true, Height = 20, Margin = new Thickness(20) },
                    new TextBlock { Text = "Создание отчета...", HorizontalAlignment = HorizontalAlignment.Center }
                }
                    }
                };

                // Запускаем в отдельном потоке чтобы не блокировать UI
                System.Threading.Tasks.Task.Run(() =>
                {
                    var filePath = _excelExportService.ExportFinancialReport(startDate, endDate, reportType);

                    // Закрываем окно прогресса и открываем файл в UI потоке
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        progressWindow.Close();

                        if (filePath != null)
                        {
                            var result = MessageBox.Show(
                                $"Отчет успешно создан!\n{filePath}\n\nОткрыть файл?",
                                "Экспорт завершен",
                                MessageBoxButton.YesNo);

                            if (result == MessageBoxResult.Yes)
                            {
                                _excelExportService.OpenFileInExcel(filePath);
                            }
                        }
                    });
                });

                progressWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}");
            }
        }

        private void ExportFullReport_Click(object sender, RoutedEventArgs e)
        {
            ExportReport("full");
        }

        private void ExportTransactions_Click(object sender, RoutedEventArgs e)
        {
            ExportReport("transactions");
        }

        private void ExportExpenses_Click(object sender, RoutedEventArgs e)
        {
            ExportReport("expenses");
        }

        private void ExportSummary_Click(object sender, RoutedEventArgs e)
        {
            ExportReport("summary");
        }
    }
}