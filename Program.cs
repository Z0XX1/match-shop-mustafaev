using Microsoft.EntityFrameworkCore;

Console.WriteLine("Магазин спичек «Чирк»: проект запущен!");

// Дальше — код по шагам задания.
// Шаг 1
using var db = new ShopContext();
db.Database.EnsureDeleted();
db.Database.EnsureCreated();

Console.WriteLine("Шаг 1");
Console.WriteLine("Таблицы созданы");

//Шаг 2
Console.WriteLine("Шаг 2");
Console.WriteLine("Таблица заказов создана");

//Шаг 3
var ivan = new Customer { Name = "Иван Петров", Phone = "+7 900 111-22-33", City = "Омск" };
var olga = new Customer { Name = "Ольга Смирнова", Phone = "+7 900 444-55-66", City = "Томск" };
var maria = new Customer { Name = "Мария Иванова", Phone = "+7 901 123-45-67", City = "Владивосток" };
var dmitry = new Customer { Name = "Дмитрий Соколов", Phone = "+7 902 234-56-78", City = "Москва" };
var sergey = new Customer { Name = "Сергей Волков", Phone = "+7 903 345-67-89", City = "Казань" };

db.Customers.AddRange(ivan, olga, maria, dmitry, sergey);

var domestic = new Product { Name = "Хозяйственные", Price = 45m, MatchesPerBox = 40, Stock = 120 };
var tourist = new Product { Name = "Туристические", Price = 89m, MatchesPerBox = 20, Stock = 40, Description = "Не боятся влаги" };
var fireplace = new Product { Name = "Каминные", Price = 150m, MatchesPerBox = 30, Stock = 10 };
var hunting = new Product { Name = "Охотничьи", Price = 230m, MatchesPerBox = 15, Stock = 25 };
var souvenir = new Product { Name = "Сувенирные", Price = 300m, MatchesPerBox = 10, Stock = 5 };

db.Products.AddRange(domestic, tourist, fireplace, hunting, souvenir);
db.SaveChanges();

db.Orders.AddRange(
    new Order
    {
        Customer = ivan,
        Product = fireplace,
        Quantity = 3,
        CreatedAt = DateTime.UtcNow.AddDays(-6)
    },

    new Order
    {
        Customer = ivan,
        Product = domestic,
        Quantity = 4,
        CreatedAt = DateTime.UtcNow.AddDays(-5)
    },

    new Order
    {
        Customer = maria,
        Product = tourist,
        Quantity = 2,
        CreatedAt = DateTime.UtcNow.AddDays(-4)
    },

    new Order
    {
        Customer = dmitry,
        Product = hunting,
        Quantity = 1,
        CreatedAt = DateTime.UtcNow.AddDays(-3)
    }
);

db.SaveChanges();
Console.WriteLine("Шаг 3");
Console.WriteLine("Покупатели, товары и заказы добавлены");
// Now возвращает местное время, UtcNow возвращает время UTC

//Шаг 4
Console.WriteLine("Шаг 4");

// Покупатели из Омска по алфавиту
var omsk = db.Customers
    .Where(c => c.City == "Омск")
    .OrderBy(c => c.Name)
    .ToList();
foreach (var c in omsk)
    Console.WriteLine($"{c.Name}, {c.Phone}");

// Товары дешевле 100 рублей
Console.WriteLine("Товары дешевле 100 рублей:");

var cheap = db.Products
    .Where(p => p.Price < 100m)
    .OrderBy(p => p.Price)
    .ToList();
foreach (var p in cheap)
    Console.WriteLine($"{p.Name} - {p.Price} руб.");

// Количество товаров на складе
int count = db.Products.Count(p => p.Stock > 0);
Console.WriteLine($"Видов товаров на складе: {count}");

// Три самых дорогих товара
Console.WriteLine("Три самых дорогих товара:");

var expensive = db.Products
    .OrderByDescending(p => p.Price)
    .Take(3)
    .ToList();
foreach (var p in expensive)
    Console.WriteLine($"{p.Name} - {p.Price} руб.");

// УСЛОЖНЕНИЕ

Console.WriteLine("Все заказы:");
var orders = db.Orders
    .Include(o => o.Customer)
    .Include(o => o.Product)
    .ToList();
decimal revenue = 0;

foreach (var order in orders)
{
    decimal total = order.Product.Price * order.Quantity;
    Console.WriteLine($"{order.Customer.Name} - {order.Product.Name} - {order.Quantity} шт. - {total} руб.");
    revenue += total;
}
Console.WriteLine($"Общая выручка: {revenue} руб.");

//Шаг 5
Console.WriteLine("Шаг 5");

// Измена телефона Ивана
var customer = db.Customers.First(c => c.Name == "Иван Петров");
customer.Phone = "+7 900 999-00-00";
db.SaveChanges();

// Увеличение цены на 10%
var allProducts = db.Products.ToList();
foreach (var product in allProducts)
{
    product.Price *= 1.1m;
}

// Сувенирные спички закончились
souvenir.Stock = 0;
db.SaveChanges();
Console.WriteLine("Цены и остатки обновлены");

// УСЛОЖНЕНИЕ

void MakeOrder(Customer buyer, Product item, int quantity)
{
    if (quantity <= 0 || item.Stock < quantity)
    {
        Console.WriteLine("Недостаточно на складе");
        return;
    }

    db.Orders.Add(new Order
    {
        Customer = buyer,
        Product = item,
        Quantity = quantity,
        CreatedAt = DateTime.UtcNow
    });

    item.Stock -= quantity;
    db.SaveChanges();
    Console.WriteLine("Заказ оформлен");
}

// Успешный заказ
MakeOrder(sergey, fireplace, 2);

// Неуспешный заказ
MakeOrder(sergey, fireplace, 1000);
// Заказ и остаток сохраняется одним сейвом чтобы оба изменения выполнились вместе иначе товар можно списать без сохранения заказа

//Шаг 6
Console.WriteLine("Шаг 6");

// Удаление ольги
var leaving = db.Customers.First(c => c.Name == "Ольга Смирнова");
db.Customers.Remove(leaving);
db.SaveChanges();

// Удаление товаров с нулевым остатком
var emptyProducts = db.Products
    .Where(p => p.Stock == 0)
    .ToList();

db.Products.RemoveRange(emptyProducts);
db.SaveChanges();
Console.WriteLine($"Осталось товаров: {db.Products.Count()}");

// УСЛОЖНЕНИЕ

// У Ивана есть два заказа
var buyer = db.Customers.First(c => c.Name == "Иван Петров");
int before = db.Orders.Count(o => o.CustomerId == buyer.Id);
db.Customers.Remove(buyer);
db.SaveChanges();
int after = db.Orders.Count(o => o.CustomerId == buyer.Id);
Console.WriteLine($"Заказов до удаления: {before}");
Console.WriteLine($"Заказов после удаления: {after}");

// Заказы ивана тоже удалились из-за каскадного удаления, это происходит из-за обязательной связи CustomerId и для магазина эт плохо т.к теряется история покупок