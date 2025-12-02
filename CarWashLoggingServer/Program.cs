var builder = WebApplication.CreateBuilder(args);

// Добавляем MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// В разработке показываем страницу ошибок
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Статические файлы (css, js)
app.UseStaticFiles();

// Маршрутизация
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();