using ShtuchkiDruchki.Models;
using ShtuchkiDruchki.Money;

namespace ShtuchkiDruchki.Discounts;

public sealed class FirstOrderDiscount : IDiscount
{
    private readonly decimal _percent;

    public string Name { get; }
    public DiscountStage Stage => DiscountStage.Percent;

    public FirstOrderDiscount(string name, decimal percent)
    {
        if (percent < 0) throw new ArgumentOutOfRangeException(nameof(percent));
        Name = name;
        _percent = percent;
    }

    public bool Apply(DiscountContext ctx)
    {
        if (!ctx.Order.Customer.IsFirstOrder) return false;

        var amount = MoneyHelper.Round(ctx.CurrentItemsTotal * _percent / 100m);
        if (amount <= 0) return false;

        ctx.ReduceItems(amount, new AppliedDiscount(
            Name, amount, "Первый заказ покупателя"));
        return true;
    }
}