using ShtuchkiDruchki.Models;
using ShtuchkiDruchki.Money;

namespace ShtuchkiDruchki.Discounts;

public sealed class PercentDiscount : IDiscount
{
    private readonly decimal _percent;

    public string Name { get; }
    public decimal Percent => _percent;
    public DiscountStage Stage => DiscountStage.Percent;

    public PercentDiscount(string name, decimal percent)
    {
        if (percent < 0) throw new ArgumentOutOfRangeException(nameof(percent));
        Name = name;
        _percent = percent;
    }

    public bool Apply(DiscountContext ctx)
    {
        var amount = MoneyHelper.Round(ctx.CurrentItemsTotal * _percent / 100m);
        if (amount <= 0) return false;

        ctx.ReduceItems(amount, new AppliedDiscount(
            Name, amount, $"Скидка {_percent}% на сумму товаров"));
        return true;
    }
}