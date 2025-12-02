using CarWashLoggingServer.Models;
using CarWashLoggingServer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CarWashLoggingServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly LogService _logService;

        public HomeController()
        {
            _logService = new LogService();
        }

        // Главная страница с логами
        public IActionResult Index(string level = null, string user = null)
        {
            var logs = _logService.GetLogs(level: level);

            if (!string.IsNullOrEmpty(user))
                logs = logs.Where(l => l.User.Contains(user, StringComparison.OrdinalIgnoreCase)).ToList();

            ViewBag.Stats = _logService.GetStats();
            ViewBag.SelectedLevel = level;
            ViewBag.SelectedUser = user;

            return View(logs);
        }

        // Страница статистики
        public IActionResult Stats()
        {
            var stats = _logService.GetStats();
            return View(stats);
        }

        // API для добавления лога (для WPF приложения)
        [HttpPost]
        public IActionResult AddLog([FromBody] LogEntry log)
        {
            try
            {
                _logService.AddLog(log.Message, log.Level, log.User, log.Component);
                return Json(new { success = true, message = "Лог создан" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Простая форма для ручного добавления лога
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(string message, string level = "Информация", string user = "", string component = "")
        {
            _logService.AddLog(message, level, user, component);
            TempData["Message"] = "Лог успешно добавлен!";
            return RedirectToAction("Index");
        }
    }
}