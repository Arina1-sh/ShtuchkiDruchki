namespace ShtuchkiDruchki.Models;

public sealed class CartItem
{
    public Product Product { get; }
    public int Quantity { get; }

    public CartItem(Product product, int quantity)
    {
        Product = product ?? throw new ArgumentNullException(nameof(product));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "quantity должно быть > 0");
        Quantity = quantity;
    }
}