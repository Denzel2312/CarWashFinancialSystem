using CarWashFinancialSystem.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarWashFinancialSystem.Views.Pages
{
    public partial class DashboardPage : Page
    {
        private readonly FinancialService _financialService;

        public DashboardPage()
        {
            InitializeComponent();
            _financialService = new FinancialService();
            ReportDatePicker.SelectedDate = DateTime.Today;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshFinancialData();
        }

        private void RefreshData_Click(object sender, RoutedEventArgs e)
        {
            RefreshFinancialData();
        }

        private void RefreshFinancialData()
        {
            try
            {
                var selectedDate = ReportDatePicker.SelectedDate ?? DateTime.Today;
                var startDate = selectedDate.Date;
                var endDate = selectedDate.Date.AddDays(1).AddSeconds(-1);

                // Получаем финансовый отчёт
                var report = _financialService.GenerateDailyReport(selectedDate);

                // Обновляем UI
                RevenueText.Text = $"{report.TotalRevenue:N0} ₽";
                ExpensesText.Text = $"{report.TotalExpenses:N0} ₽";
                ProfitText.Text = $"{report.NetProfit:N0} ₽";
                ProfitMarginText.Text = $"Рентабельность: {report.ProfitMargin:F1}%";

                // Детализация расходов
                WaterExpenseText.Text = $"{report.WaterExpenses:N0} ₽";
                ElectricityExpenseText.Text = $"{report.ElectricityExpenses:N0} ₽";
                HeatingExpenseText.Text = $"{report.HeatingExpenses:N0} ₽";
                SalaryExpenseText.Text = $"{report.SalaryExpenses:N0} ₽";

                // Расходы на одну машину
                var waterPerCar = _financialService.CalculateWaterCostPerCar(startDate, endDate);
                var electricityPerCar = _financialService.CalculateElectricityCostPerCar(startDate, endDate);
                WaterPerCarText.Text = $"{waterPerCar:F1} ₽/авто";
                ElectricityPerCarText.Text = $"{electricityPerCar:F1} ₽/авто";

                // Статистика услуг
                ServicesCountText.Text = report.ServicesCount.ToString();
                AverageCheckText.Text = $"{report.AverageCheck:F0} ₽";

                // Самая популярная услуга
                var popularity = _financialService.GetServicePopularityForPeriod(startDate, endDate);
                if (popularity.Any())
                {
                    var popularService = popularity.OrderByDescending(x => x.Value).First();
                    PopularServiceText.Text = popularService.Key;
                    PopularServiceCountText.Text = $"{popularService.Value} раз";
                }
                else
                {
                    PopularServiceText.Text = "Нет данных";
                    PopularServiceCountText.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}