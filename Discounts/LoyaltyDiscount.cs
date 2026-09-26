using ShtuchkiDruchki.Models;
using ShtuchkiDruchki.Money;

namespace ShtuchkiDruchki.Discounts;

public sealed class LoyaltyDiscount : IDiscount
{
    private readonly decimal _percent;
    private readonly int _days;

    public string Name { get; }
    public DiscountStage Stage => DiscountStage.Percent;

    public LoyaltyDiscount(string name, decimal percent, int days)
    {
        if (percent < 0) throw new ArgumentOutOfRangeException(nameof(percent));
        if (days < 0) throw new ArgumentOutOfRangeException(nameof(days));

        Name = name;
        _percent = percent;
        _days = days;
    }

    public bool Apply(DiscountContext ctx)
    {
        var last = ctx.Order.Customer.LastPurchaseDate;
        if (last is null) return false;

        var diff = (ctx.Order.CreatedAt.Date - last.Value.Date).Days;
        if (diff < 0 || diff > _days) return false;

        var amount = MoneyHelper.Round(ctx.CurrentItemsTotal * _percent / 100m);
        if (amount <= 0) return false;

        ctx.ReduceItems(amount, new AppliedDiscount(
            Name, amount,
            $"Последняя покупка была {diff} дней назад"));
        return true;
    }
}