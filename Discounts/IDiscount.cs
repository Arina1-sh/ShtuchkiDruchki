using ShtuchkiDruchki.Models;

namespace ShtuchkiDruchki.Discounts;

public enum DiscountStage
{
    Quantity = 0,
    Percent = 1,
    Fixed = 2,
    Delivery = 3
}

public interface IDiscount
{
    string Name { get; }
    DiscountStage Stage { get; }

    bool Apply(DiscountContext context);
}