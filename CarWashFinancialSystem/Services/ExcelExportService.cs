using CarWashFinancialSystem.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace CarWashFinancialSystem.Services
{
    public class ExcelExportService
    {
        private readonly TransactionService _transactionService;
        private readonly ExpenseService _expenseService;
        private readonly ReportService _reportService;

        public ExcelExportService()
        {
            _transactionService = new TransactionService();
            _expenseService = new ExpenseService();
            _reportService = new ReportService();

            // Устанавливаем лицензию EPPlus (бесплатная для некоммерческого использования)
            ExcelPackage.License.SetNonCommercialPersonal("My Name");
        }
        private class FinancialIndicator
        {
            public string Name { get; set; }
            public decimal Value { get; set; }
            public string Format { get; set; }
            public System.Drawing.Color Color { get; set; }
        }
        public string ExportFinancialReport(DateTime startDate, DateTime endDate, string reportType = "full")
        {
            try
            {
                var fileName = $"Отчет_автомойка_{startDate:ddMMyyyy}_{endDate:ddMMyyyy}.xlsx";
                var filePath = Path.Combine(Path.GetTempPath(), fileName);

                using (var package = new ExcelPackage())
                {
                    switch (reportType.ToLower())
                    {
                        case "transactions":
                            var transSheet = package.Workbook.Worksheets.Add("Транзакции");
                            CreateTransactionsSheet(transSheet, startDate, endDate);
                            break;
                        case "expenses":
                            var expSheet = package.Workbook.Worksheets.Add("Расходы");
                            CreateExpensesSheet(expSheet, startDate, endDate);
                            break;
                        case "summary":
                            var sumSheet = package.Workbook.Worksheets.Add("Сводка");
                            CreateSummarySheet(sumSheet, startDate, endDate);
                            break;
                        default:
                            CreateFullReport(package, startDate, endDate);
                            break;
                    }

                    package.SaveAs(new FileInfo(filePath));
                }

                return filePath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте в Excel: {ex.Message}");
                return null;
            }
        }
        private void CreateFullReport(ExcelPackage package, DateTime startDate, DateTime endDate)
        {
            // Титульная страница
            var titleSheet = package.Workbook.Worksheets.Add("Общая информация");
            CreateTitlePage(titleSheet, startDate, endDate);

            // Финансовые показатели
            var financialSheet = package.Workbook.Worksheets.Add("Финансовые показатели");
            CreateSummarySheet(financialSheet, startDate, endDate);

            // Транзакции
            var transactionsSheet = package.Workbook.Worksheets.Add("Транзакции");
            CreateTransactionsSheet(transactionsSheet, startDate, endDate);

            // Расходы
            var expensesSheet = package.Workbook.Worksheets.Add("Расходы");
            CreateExpensesSheet(expensesSheet, startDate, endDate);

            // Аналитика
            var analyticsSheet = package.Workbook.Worksheets.Add("Аналитика");
            CreateAnalyticsSheet(analyticsSheet, startDate, endDate);
        }

        private void CreateTitlePage(ExcelWorksheet sheet, DateTime startDate, DateTime endDate)
        {
            // Заголовок
            sheet.Cells["A1"].Value = "ФИНАНСОВЫЙ ОТЧЕТ";
            sheet.Cells["A1"].Style.Font.Size = 16;
            sheet.Cells["A1"].Style.Font.Bold = true;
            sheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells["A1:D1"].Merge = true;

            // Период
            sheet.Cells["A3"].Value = "Период:";
            sheet.Cells["B3"].Value = $"{startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}";
            sheet.Cells["B3"].Style.Font.Bold = true;

            // Дата генерации
            sheet.Cells["A4"].Value = "Дата генерации:";
            sheet.Cells["B4"].Value = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

            // Сводная статистика
            var summary = _reportService.GetFinancialSummary(startDate, endDate);

            sheet.Cells["A6"].Value = "ВЫРУЧКА:";
            sheet.Cells["B6"].Value = summary.TotalRevenue;
            sheet.Cells["B6"].Style.Numberformat.Format = "#,##0.00\" руб\"";
            sheet.Cells["B6"].Style.Font.Bold = true;
            sheet.Cells["B6"].Style.Font.Color.SetColor(System.Drawing.Color.Green);

            sheet.Cells["A7"].Value = "РАСХОДЫ:";
            sheet.Cells["B7"].Value = summary.TotalExpenses;
            sheet.Cells["B7"].Style.Numberformat.Format = "#,##0.00\" руб\"";
            sheet.Cells["B7"].Style.Font.Bold = true;
            sheet.Cells["B7"].Style.Font.Color.SetColor(System.Drawing.Color.Red);

            sheet.Cells["A8"].Value = "ПРИБЫЛЬ:";
            sheet.Cells["B8"].Value = summary.Profit;
            sheet.Cells["B8"].Style.Numberformat.Format = "#,##0.00\" руб\"";
            sheet.Cells["B8"].Style.Font.Bold = true;
            sheet.Cells["B8"].Style.Font.Color.SetColor(System.Drawing.Color.Blue);

            sheet.Cells["A9"].Value = "КОЛИЧЕСТВО ОПЕРАЦИЙ:";
            sheet.Cells["B9"].Value = summary.TransactionCount;
            sheet.Cells["B9"].Style.Font.Bold = true;

            sheet.Cells["A10"].Value = "СРЕДНИЙ ЧЕК:";
            sheet.Cells["B10"].Value = summary.AverageCheck;
            sheet.Cells["B10"].Style.Numberformat.Format = "#,##0.00\" руб\"";
            sheet.Cells["B10"].Style.Font.Bold = true;

            // Автоматическое выравнивание столбцов
            sheet.Cells[1, 1, 10, 2].AutoFitColumns();
        }


        private void CreateTransactionsSheet(ExcelWorksheet sheet, DateTime startDate, DateTime endDate)
        {

            // Заголовок
            sheet.Cells["A1"].Value = "ТРАНЗАКЦИИ";
            sheet.Cells["A1"].Style.Font.Size = 14;
            sheet.Cells["A1"].Style.Font.Bold = true;
            sheet.Cells["A1:H1"].Merge = true;

            // Заголовки столбцов
            var headers = new[] { "Дата", "Услуга", "Тип авто", "Госномер", "Сумма", "Способ оплаты", "Оператор", "Примечания" };
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cells[3, i + 1].Value = headers[i];
                sheet.Cells[3, i + 1].Style.Font.Bold = true;
                sheet.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Данные
            var transactions = _transactionService.GetAllTransactions()
                .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .OrderByDescending(t => t.TransactionDate);

            int row = 4;
            foreach (var transaction in transactions)
            {
                sheet.Cells[row, 1].Value = transaction.TransactionDate;
                sheet.Cells[row, 1].Style.Numberformat.Format = "dd.MM.yyyy HH:mm";
                sheet.Cells[row, 2].Value = transaction.Service?.Name;
                sheet.Cells[row, 3].Value = transaction.CarType;
                sheet.Cells[row, 4].Value = transaction.LicensePlate;
                sheet.Cells[row, 5].Value = transaction.Amount;
                sheet.Cells[row, 5].Style.Numberformat.Format = "#,##0.00\" руб\"";
                sheet.Cells[row, 6].Value = transaction.PaymentMethod;
                sheet.Cells[row, 7].Value = transaction.Operator?.FullName;
                sheet.Cells[row, 8].Value = transaction.Notes;
                row++;
            }

            // Итоговая строка
            sheet.Cells[row, 4].Value = "ИТОГО:";
            sheet.Cells[row, 4].Style.Font.Bold = true;
            sheet.Cells[row, 5].Formula = $"SUM(E4:E{row - 1})";
            sheet.Cells[row, 5].Style.Numberformat.Format = "#,##0.00\" руб\"";
            sheet.Cells[row, 5].Style.Font.Bold = true;
            sheet.Cells[row, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[row, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);

            // Границы и авто-размеры
            sheet.Cells[3, 1, row, 8].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            sheet.Cells[3, 1, row, 8].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            sheet.Cells[3, 1, row, 8].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[3, 1, row, 8].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            sheet.Cells[1, 1, row, 8].AutoFitColumns();
        }

        private void CreateExpensesSheet(ExcelWorksheet sheet, DateTime startDate, DateTime endDate)
        {

            // Заголовок
            sheet.Cells["A1"].Value = "РАСХОДЫ";
            sheet.Cells["A1"].Style.Font.Size = 14;
            sheet.Cells["A1"].Style.Font.Bold = true;
            sheet.Cells["A1:E1"].Merge = true;

            // Заголовки столбцов
            var headers = new[] { "Дата", "Категория", "Описание", "Сумма", "Примечания" };
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cells[3, i + 1].Value = headers[i];
                sheet.Cells[3, i + 1].Style.Font.Bold = true;
                sheet.Cells[3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Данные
            var expenses = _expenseService.GetAllExpenses()
                .Where(e => e.ExpenseDate >= startDate && e.ExpenseDate <= endDate)
                .OrderByDescending(e => e.ExpenseDate);

            int row = 4;
            foreach (var expense in expenses)
            {
                sheet.Cells[row, 1].Value = expense.ExpenseDate;
                sheet.Cells[row, 1].Style.Numberformat.Format = "dd.MM.yyyy";
                sheet.Cells[row, 2].Value = expense.Category;
                sheet.Cells[row, 3].Value = expense.Description;
                sheet.Cells[row, 4].Value = expense.Amount;
                sheet.Cells[row, 4].Style.Numberformat.Format = "#,##0.00\" руб\"";
                sheet.Cells[row, 5].Value = expense.Notes;
                row++;
            }

            // Итоговая строка
            sheet.Cells[row, 3].Value = "ИТОГО:";
            sheet.Cells[row, 3].Style.Font.Bold = true;
            sheet.Cells[row, 4].Formula = $"SUM(D4:D{row - 1})";
            sheet.Cells[row, 4].Style.Numberformat.Format = "#,##0.00\" руб\"";
            sheet.Cells[row, 4].Style.Font.Bold = true;
            sheet.Cells[row, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[row, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCoral);

            // Границы и авто-размеры
            sheet.Cells[3, 1, row, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            sheet.Cells[3, 1, row, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            sheet.Cells[3, 1, row, 5].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[3, 1, row, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            sheet.Cells[1, 1, row, 5].AutoFitColumns();
        }

        private void CreateAnalyticsSheet(ExcelWorksheet sheet, DateTime startDate, DateTime endDate)
        {

            // Заголовок
            sheet.Cells["A1"].Value = "АНАЛИТИКА";
            sheet.Cells["A1"].Style.Font.Size = 14;
            sheet.Cells["A1"].Style.Font.Bold = true;
            sheet.Cells["A1:D1"].Merge = true;

            // Статистика по услугам
            sheet.Cells["A3"].Value = "СТАТИСТИКА ПО УСЛУГАМ";
            sheet.Cells["A3"].Style.Font.Bold = true;
            sheet.Cells["A3:D3"].Merge = true;

            var serviceStats = _reportService.GetServiceStatistics(startDate, endDate);

            sheet.Cells["A4"].Value = "Услуга";
            sheet.Cells["B4"].Value = "Количество";
            sheet.Cells["C4"].Value = "Сумма";
            sheet.Cells["D4"].Value = "Доля";
            for (int i = 0; i < 4; i++)
            {
                sheet.Cells[4, i + 1].Style.Font.Bold = true;
                sheet.Cells[4, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[4, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            }

            int row = 5;
            foreach (var stat in serviceStats)
            {
                sheet.Cells[row, 1].Value = stat.ServiceName;
                sheet.Cells[row, 2].Value = stat.Count;
                sheet.Cells[row, 3].Value = stat.TotalAmount;
                sheet.Cells[row, 3].Style.Numberformat.Format = "#,##0.00\" руб\"";
                sheet.Cells[row, 4].Value = stat.Percentage / 100; // Excel ожидает долю, а не процент
                sheet.Cells[row, 4].Style.Numberformat.Format = "0.0%";
                row++;
            }

            // Ежедневная статистика
            sheet.Cells[$"A{row + 2}"].Value = "ЕЖЕДНЕВНАЯ СТАТИСТИКА";
            sheet.Cells[$"A{row + 2}"].Style.Font.Bold = true;
            sheet.Cells[$"A{row + 2}:E{row + 2}"].Merge = true;

            var dailyReport = _reportService.GetDailyReport(startDate, endDate);

            sheet.Cells[$"A{row + 3}"].Value = "Дата";
            sheet.Cells[$"B{row + 3}"].Value = "Выручка";
            sheet.Cells[$"C{row + 3}"].Value = "Расходы";
            sheet.Cells[$"D{row + 3}"].Value = "Прибыль";
            sheet.Cells[$"E{row + 3}"].Value = "Операций";
            for (int i = 0; i < 5; i++)
            {
                sheet.Cells[row + 3, i + 1].Style.Font.Bold = true;
                sheet.Cells[row + 3, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[row + 3, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
            }

            int dailyRow = row + 4;
            foreach (var day in dailyReport)
            {
                sheet.Cells[dailyRow, 1].Value = day.Date;
                sheet.Cells[dailyRow, 1].Style.Numberformat.Format = "dd.MM.yyyy";
                sheet.Cells[dailyRow, 2].Value = day.Revenue;
                sheet.Cells[dailyRow, 2].Style.Numberformat.Format = "#,##0.00\" руб\"";
                sheet.Cells[dailyRow, 3].Value = day.Expenses;
                sheet.Cells[dailyRow, 3].Style.Numberformat.Format = "#,##0.00\" руб\"";
                sheet.Cells[dailyRow, 4].Value = day.Profit;
                sheet.Cells[dailyRow, 4].Style.Numberformat.Format = "#,##0.00\" руб\"";
                sheet.Cells[dailyRow, 5].Value = day.TransactionsCount;
                dailyRow++;
            }

            sheet.Cells[1, 1, dailyRow, 5].AutoFitColumns();
        }

        private void CreateSummarySheet(ExcelWorksheet sheet, DateTime startDate, DateTime endDate)
        {

            // Заголовок
            sheet.Cells["A1"].Value = "ФИНАНСОВАЯ СВОДКА";
            sheet.Cells["A1"].Style.Font.Size = 16;
            sheet.Cells["A1"].Style.Font.Bold = true;
            sheet.Cells["A1:C1"].Merge = true;

            // Период
            sheet.Cells["A3"].Value = "Период отчета:";
            sheet.Cells["B3"].Value = $"{startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}";
            sheet.Cells["B3"].Style.Font.Bold = true;

            // Основные финансовые показатели
            var summary = _reportService.GetFinancialSummary(startDate, endDate);

            sheet.Cells["A5"].Value = "ОСНОВНЫЕ ПОКАЗАТЕЛИ";
            sheet.Cells["A5"].Style.Font.Bold = true;
            sheet.Cells["A5:C5"].Merge = true;

            // Создаем класс для индикаторов
            var indicators = new List<FinancialIndicator>
            {
                new FinancialIndicator { Name = "Общая выручка", Value = summary.TotalRevenue, Format = "#,##0.00\" руб\"", Color = System.Drawing.Color.Green },
                new FinancialIndicator { Name = "Общие расходы", Value = summary.TotalExpenses, Format = "#,##0.00\" руб\"", Color = System.Drawing.Color.Red },
                new FinancialIndicator { Name = "Чистая прибыль", Value = summary.Profit, Format = "#,##0.00\" руб\"", Color = System.Drawing.Color.Blue },
                new FinancialIndicator { Name = "Количество операций", Value = (decimal)summary.TransactionCount, Format = "0", Color = System.Drawing.Color.Black },
                new FinancialIndicator { Name = "Средний чек", Value = summary.AverageCheck, Format = "#,##0.00\" руб\"", Color = System.Drawing.Color.Orange },
                new FinancialIndicator { Name = "Рентабельность", Value = summary.TotalRevenue > 0 ? (summary.Profit / summary.TotalRevenue) * 100 : 0, Format = "0.0%", Color = System.Drawing.Color.Purple }
            };

            int row = 6;
            foreach (var indicator in indicators)
            {
                sheet.Cells[row, 1].Value = indicator.Name;
                sheet.Cells[row, 2].Value = indicator.Value;
                sheet.Cells[row, 2].Style.Numberformat.Format = indicator.Format;
                sheet.Cells[row, 2].Style.Font.Bold = true;
                sheet.Cells[row, 2].Style.Font.Color.SetColor(indicator.Color);
                row++;
            }

            // Статистика по услугам
            row += 2;
            sheet.Cells[row, 1].Value = "СТАТИСТИКА ПО УСЛУГАМ";
            sheet.Cells[row, 1].Style.Font.Bold = true;
            sheet.Cells[row, 1].Style.Font.Size = 12;
            sheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            sheet.Cells[row, 1, row, 4].Merge = true;

            row++;
            sheet.Cells[row, 1].Value = "Услуга";
            sheet.Cells[row, 2].Value = "Количество";
            sheet.Cells[row, 3].Value = "Сумма";
            sheet.Cells[row, 4].Value = "Доля";
            for (int i = 1; i <= 4; i++)
            {
                sheet.Cells[row, i].Style.Font.Bold = true;
                sheet.Cells[row, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[row, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            }

            var serviceStats = _reportService.GetServiceStatistics(startDate, endDate);
            row++;
            foreach (var stat in serviceStats.Take(10)) // Показываем топ-10 услуг
            {
                sheet.Cells[row, 1].Value = stat.ServiceName;
                sheet.Cells[row, 2].Value = stat.Count;
                sheet.Cells[row, 3].Value = stat.TotalAmount;
                sheet.Cells[row, 3].Style.Numberformat.Format = "#,##0.00\" руб\"";
                sheet.Cells[row, 4].Value = stat.Percentage / 100;
                sheet.Cells[row, 4].Style.Numberformat.Format = "0.0%";
                row++;
            }

            // Ежедневная статистика
            row += 2;
            sheet.Cells[row, 1].Value = "ЕЖЕДНЕВНАЯ СТАТИСТИКА";
            sheet.Cells[row, 1].Style.Font.Bold = true;
            sheet.Cells[row, 1].Style.Font.Size = 12;
            sheet.Cells[row, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            sheet.Cells[row, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
            sheet.Cells[row, 1, row, 5].Merge = true;

            row++;
            sheet.Cells[row, 1].Value = "Дата";
            sheet.Cells[row, 2].Value = "Выручка";
            sheet.Cells[row, 3].Value = "Расходы";
            sheet.Cells[row, 4].Value = "Прибыль";
            sheet.Cells[row, 5].Value = "Операций";
            for (int i = 1; i <= 5; i++)
            {
                sheet.Cells[row, i].Style.Font.Bold = true;
                sheet.Cells[row, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[row, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
            }

            var dailyReport = _reportService.GetDailyReport(startDate, endDate);
            row++;
            foreach (var day in dailyReport.Take(14)) // Показываем последние 14 дней
            {
                sheet.Cells[row, 1].Value = day.Date;
                sheet.Cells[row, 1].Style.Numberformat.Format = "dd.MM.yyyy";
                sheet.Cells[row, 2].Value = day.Revenue;
                sheet.Cells[row, 2].Style.Numberformat.Format = "#,##0.00\" руб\"";
                sheet.Cells[row, 3].Value = day.Expenses;
                sheet.Cells[row, 3].Style.Numberformat.Format = "#,##0.00\" руб\"";
                sheet.Cells[row, 4].Value = day.Profit;
                sheet.Cells[row, 4].Style.Numberformat.Format = "#,##0.00\" руб\"";
                sheet.Cells[row, 5].Value = day.TransactionsCount;
                row++;
            }

            // Автоматическое выравнивание столбцов
            sheet.Cells[1, 1, row, 5].AutoFitColumns();

            // Границы для таблиц
            sheet.Cells[6, 1, 6 + indicators.Count - 1, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            sheet.Cells[6, 1, 6 + indicators.Count - 1, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            sheet.Cells[6, 1, 6 + indicators.Count - 1, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[6, 1, 6 + indicators.Count - 1, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }

        public void OpenFileInExcel(string filePath)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть файл: {ex.Message}");
            }
        }
    }
}