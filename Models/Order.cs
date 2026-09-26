namespace ShtuchkiDruchki.Models;

public sealed class Order
{
    public string Id { get; init; } = default!;
    public Customer Customer { get; init; } = default!;
    public IReadOnlyList<CartItem> Items { get; init; } = Array.Empty<CartItem>();
    public string? PromoCode { get; init; }
    public DateTime CreatedAt { get; init; }
    public decimal DeliveryCost { get; init; }

    public Order(string id, Customer customer, IEnumerable<CartItem> items,
                 string? promoCode, DateTime createdAt, decimal deliveryCost)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("id обязателен", nameof(id));

        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        Items = (items ?? throw new ArgumentNullException(nameof(items))).ToList();

        if (deliveryCost < 0)
            throw new ArgumentOutOfRangeException(nameof(deliveryCost), "Стоимость доставки не может быть отрицательной");

        Id = id;
        PromoCode = promoCode;
        CreatedAt = createdAt;
        DeliveryCost = deliveryCost;
    }
}