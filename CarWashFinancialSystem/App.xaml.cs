using CarWashFinancialSystem.Services;
using System.Configuration;
using System.Data;
using System.Windows;

namespace CarWashFinancialSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static LoggingService Logger { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Инициализируем логгер
            Logger = new LoggingService();

            // Логируем запуск приложения (СИНХРОННО)
            Logger.LogSystem("CarWash Financial System запущено");

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Логируем завершение приложения (СИНХРОННО)
            Logger.LogSystem("CarWash Financial System остановлено");

            base.OnExit(e);
        }
    }

}
