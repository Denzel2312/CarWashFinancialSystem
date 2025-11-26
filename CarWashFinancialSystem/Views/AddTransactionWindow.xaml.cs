using CarWashFinancialSystem.Models;
using CarWashFinancialSystem.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarWashFinancialSystem.Views
{
    public partial class AddTransactionWindow : Window
    {
        private readonly TransactionService _transactionService;
        private readonly ServiceService _serviceService;

        public event EventHandler TransactionAdded;

        public AddTransactionWindow()
        {
            InitializeComponent();
            _transactionService = new TransactionService();
            _serviceService = new ServiceService();
            LoadServices();

            // Установка значений по умолчанию
            CarTypeComboBox.SelectedIndex = 0;
            PaymentMethodComboBox.SelectedIndex = 0;
        }

        private void LoadServices()
        {
            var services = _serviceService.GetAllServices();
            ServiceComboBox.ItemsSource = services;
            if (services.Count > 0)
                ServiceComboBox.SelectedIndex = 0;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceComboBox.SelectedItem is not Service selectedService)
            {
                MessageBox.Show("Выберите услугу");
                return;
            }

            if (CarTypeComboBox.SelectedItem is not ComboBoxItem selectedCarType)
            {
                MessageBox.Show("Выберите тип автомобиля");
                return;
            }

            if (PaymentMethodComboBox.SelectedItem is not ComboBoxItem selectedPaymentMethod)
            {
                MessageBox.Show("Выберите способ оплаты");
                return;
            }

            bool success = _transactionService.AddTransaction(
                serviceId: selectedService.Id,
                amount: selectedService.Price,
                carType: selectedCarType.Content.ToString(),
                licensePlate: LicensePlateTextBox.Text.Trim(),
                paymentMethod: selectedPaymentMethod.Content.ToString(),
                notes: NotesTextBox.Text.Trim()
            );

            if (success)
            {
                MessageBox.Show("Транзакция успешно добавлена");
                TransactionAdded?.Invoke(this, EventArgs.Empty);
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка при добавлении транзакции");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}