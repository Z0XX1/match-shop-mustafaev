// Классы таблиц: Customer, Product, Order (шаги 1–2).

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";
    public string City { get; set; } = "";
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int MatchesPerBox { get; set; }
    public int Stock { get; set; }
    public string? Description { get; set; } // Name обязателен а Desc может быть NULL потому-что его тип string? допускает отсутствие значения
}

public class Order
{
    public int Id { get; set; }
    public int Quantity { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public DateTime CreatedAt { get; set; } // DateTime хранит дату и время, в postgre эот timestamp with time zone
}