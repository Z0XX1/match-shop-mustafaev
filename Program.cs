using Microsoft.EntityFrameworkCore;

Console.WriteLine("Магазин спичек «Чирк»: проект запущен!");

// Дальше — код по шагам задания.
// Шаг 1
using var db = new ShopContext();
db.Database.EnsureDeleted();
db.Database.EnsureCreated();

Console.WriteLine("Шаг 1");
Console.WriteLine("Таблицы созданы");