using CarWashFinancialSystem.Models;
using CarWashFinancialSystem.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace CarWashFinancialSystem.Views.Pages
{
    public partial class ServicesPage : Page
    {
        private readonly ServiceService _serviceService;
        private ObservableCollection<Service> _services;
        private Service _selectedService;

        public ServicesPage()
        {
            InitializeComponent();
            _serviceService = new ServiceService();
            LoadServices();
        }

        private void LoadServices()
        {
            _services = _serviceService.GetAllServices();
            ServicesListView.ItemsSource = _services;
        }

        private void AddServiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ServiceNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(ServicePriceTextBox.Text) ||
                string.IsNullOrWhiteSpace(ServiceDurationTextBox.Text))
            {
                MessageBox.Show("Заполните все обязательные поля");
                return;
            }

            if (!decimal.TryParse(ServicePriceTextBox.Text, out decimal price) ||
                !int.TryParse(ServiceDurationTextBox.Text, out int duration))
            {
                MessageBox.Show("Проверьте правильность ввода цены и длительности");
                return;
            }

            bool success = _serviceService.AddService(
                ServiceNameTextBox.Text.Trim(),
                price,
                ServiceDescriptionTextBox.Text.Trim(),
                duration
            );

            if (success)
            {
                MessageBox.Show("Услуга успешно добавлена");
                LoadServices();
                ClearForm();
            }
            else
            {
                MessageBox.Show("Ошибка при добавлении услуги");
            }
        }

        private void UpdateServiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedService == null) return;

            if (string.IsNullOrWhiteSpace(ServiceNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(ServicePriceTextBox.Text) ||
                string.IsNullOrWhiteSpace(ServiceDurationTextBox.Text))
            {
                MessageBox.Show("Заполните все обязательные поля");
                return;
            }

            if (!decimal.TryParse(ServicePriceTextBox.Text, out decimal price) ||
                !int.TryParse(ServiceDurationTextBox.Text, out int duration))
            {
                MessageBox.Show("Проверьте правильность ввода цены и длительности");
                return;
            }

            bool success = _serviceService.UpdateService(
                _selectedService.Id,
                ServiceNameTextBox.Text.Trim(),
                price,
                ServiceDescriptionTextBox.Text.Trim(),
                duration
            );

            if (success)
            {
                MessageBox.Show("Услуга успешно обновлена");
                LoadServices();
                ClearForm();
                UpdateServiceButton.IsEnabled = false;
                AddServiceButton.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Ошибка при обновлении услуги");
            }
        }

        private void DeleteServiceButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int serviceId)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить эту услугу?",
                    "Подтверждение удаления", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    bool success = _serviceService.DeleteService(serviceId);
                    if (success)
                    {
                        MessageBox.Show("Услуга успешно удалена");
                        LoadServices();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении услуги");
                    }
                }
            }
        }

        private void EditServiceButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int serviceId)
            {
                _selectedService = _serviceService.GetServiceById(serviceId);
                if (_selectedService != null)
                {
                    ServiceNameTextBox.Text = _selectedService.Name;
                    ServicePriceTextBox.Text = _selectedService.Price.ToString();
                    ServiceDescriptionTextBox.Text = _selectedService.Description;
                    ServiceDurationTextBox.Text = _selectedService.DurationMinutes.ToString();

                    UpdateServiceButton.IsEnabled = true;
                    AddServiceButton.IsEnabled = false;
                }
            }
        }

        private void ServicesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Можно добавить функционал при выборе из списка
        }

        private void ClearFormButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            ServiceNameTextBox.Text = "";
            ServicePriceTextBox.Text = "";
            ServiceDescriptionTextBox.Text = "";
            ServiceDurationTextBox.Text = "30";
            _selectedService = null;
            UpdateServiceButton.IsEnabled = false;
            AddServiceButton.IsEnabled = true;
        }
    }
}