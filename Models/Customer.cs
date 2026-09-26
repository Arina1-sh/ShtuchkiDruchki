namespace ShtuchkiDruchki.Models;

public sealed class Customer
{
    public string Id { get; init; } = default!;
    public DateTime? LastPurchaseDate { get; init; }
    public bool IsFirstOrder { get; init; }

    public Customer(string id, DateTime? lastPurchaseDate, bool isFirstOrder)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("id обязателен", nameof(id));

        Id = id;
        LastPurchaseDate = lastPurchaseDate;
        IsFirstOrder = isFirstOrder;
    }
}