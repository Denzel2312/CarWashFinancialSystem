using CarWashFinancialSystem.Models;
using CarWashFinancialSystem.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarWashFinancialSystem.Views.Pages
{
    public partial class ExpensesPage : Page
    {
        private readonly ExpenseService _expenseService;
        private Expense _currentExpense;

        public ExpensesPage()
        {
            InitializeComponent();
            _expenseService = new ExpenseService();
            LoadExpenses();
            InitializeForm();
            ClearForm();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadExpenses();
        }

        private void InitializeForm()
        {
            // Заполняем комбобокс категориями
            CategoryComboBox.ItemsSource = ExpenseCategories.AllCategories;
            CategoryComboBox.SelectedIndex = 0;

            // Устанавливаем текущую дату
            ExpenseDatePicker.SelectedDate = DateTime.Today;
        }

        private void LoadExpenses()
        {
            var expenses = _expenseService.GetAllExpenses();
            ExpensesListView.ItemsSource = expenses;
        }

        private void ClearForm()
        {
            _currentExpense = null;
            CategoryComboBox.SelectedIndex = 0;
            DescriptionTextBox.Text = "";
            AmountTextBox.Text = "";
            ExpenseDatePicker.SelectedDate = DateTime.Today;
            NotesTextBox.Text = "";
            FormTitleText.Text = "Добавить расход";
            SaveButton.Content = "💾 Сохранить расход";
            CancelButton.Visibility = Visibility.Collapsed;
        }

        private void SaveExpense_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ||
                    string.IsNullOrWhiteSpace(AmountTextBox.Text) ||
                    CategoryComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Заполните все обязательные поля", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(AmountTextBox.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Введите корректную сумму", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_currentExpense == null)
                {
                    // Создание нового расхода
                    var newExpense = new Expense
                    {
                        Category = CategoryComboBox.SelectedItem.ToString(),
                        Description = DescriptionTextBox.Text.Trim(),
                        Amount = amount,
                        ExpenseDate = ExpenseDatePicker.SelectedDate ?? DateTime.Today,
                        Notes = NotesTextBox.Text.Trim()
                    };

                    if (_expenseService.CreateExpense(newExpense))
                    {
                        MessageBox.Show("Расход успешно добавлен", "Успех",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при добавлении расхода", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Обновление существующего расхода
                    _currentExpense.Category = CategoryComboBox.SelectedItem.ToString();
                    _currentExpense.Description = DescriptionTextBox.Text.Trim();
                    _currentExpense.Amount = amount;
                    _currentExpense.ExpenseDate = ExpenseDatePicker.SelectedDate ?? DateTime.Today;
                    _currentExpense.Notes = NotesTextBox.Text.Trim();

                    if (_expenseService.UpdateExpense(_currentExpense))
                    {
                        MessageBox.Show("Расход успешно обновлён", "Успех",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обновлении расхода", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                LoadExpenses();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditExpense_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int expenseId = (int)button.Tag;

            _currentExpense = _expenseService.GetAllExpenses().FirstOrDefault(e => e.Id == expenseId);
            if (_currentExpense != null)
            {
                CategoryComboBox.SelectedItem = _currentExpense.Category;
                DescriptionTextBox.Text = _currentExpense.Description;
                AmountTextBox.Text = _currentExpense.Amount.ToString();
                ExpenseDatePicker.SelectedDate = _currentExpense.ExpenseDate;
                NotesTextBox.Text = _currentExpense.Notes;

                FormTitleText.Text = "Редактировать расход";
                SaveButton.Content = "💾 Обновить расход";
                CancelButton.Visibility = Visibility.Visible;
            }
        }

        private void DeleteExpense_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int expenseId = (int)button.Tag;

            var result = MessageBox.Show("Вы уверены, что хотите удалить этот расход?",
                                       "Подтверждение удаления",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (_expenseService.DeleteExpense(expenseId))
                {
                    MessageBox.Show("Расход успешно удалён", "Успех",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadExpenses();

                    // Если редактировали удаляемый расход - очищаем форму
                    if (_currentExpense?.Id == expenseId)
                    {
                        ClearForm();
                    }
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении расхода", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelEdit_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }
    }
}