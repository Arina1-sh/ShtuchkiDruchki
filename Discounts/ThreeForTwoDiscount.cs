using ShtuchkiDruchki.Models;
using ShtuchkiDruchki.Money;

namespace ShtuchkiDruchki.Discounts;

public sealed class ThreeForTwoDiscount : IDiscount
{
    private readonly string _category;

    public string Name { get; }
    public DiscountStage Stage => DiscountStage.Quantity;

    public ThreeForTwoDiscount(string name, string category)
    {
        Name = name;
        _category = category ?? throw new ArgumentNullException(nameof(category));
    }

    public bool Apply(DiscountContext ctx)
    {
        var units = new List<decimal>();
        foreach (var item in ctx.Order.Items)
        {
            if (!string.Equals(item.Product.Category, _category, StringComparison.OrdinalIgnoreCase))
                continue;

            for (int i = 0; i < item.Quantity; i++)
                units.Add(item.Product.BasePrice);
        }

        var freeCount = units.Count / 3;
        if (freeCount == 0) return false;

        units.Sort();
        var amount = MoneyHelper.Round(units.Take(freeCount).Sum());
        if (amount <= 0) return false;

        ctx.ReduceItems(amount, new AppliedDiscount(
            Name, amount,
            $"Категория {_category}: {units.Count} ед., {freeCount} бесплатно"));
        return true;
    }
}