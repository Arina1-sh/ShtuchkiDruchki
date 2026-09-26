namespace ShtuchkiDruchki.Models;

public sealed class Product
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = default!;
    public decimal BasePrice { get; init; }
    public string Category { get; init; } = default!;

    public Product(string id, string name, decimal basePrice, string category)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("id обязателен", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name обязателен", nameof(name));
        if (basePrice < 0) throw new ArgumentOutOfRangeException(nameof(basePrice), "Цена не может быть отрицательной");
        if (string.IsNullOrWhiteSpace(category)) throw new ArgumentException("category обязателен", nameof(category));

        Id = id; Name = name; BasePrice = basePrice; Category = category;
    }
}