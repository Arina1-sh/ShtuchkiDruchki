using ShtuchkiDruchki.Models;
using ShtuchkiDruchki.Money;

namespace ShtuchkiDruchki.Discounts;

public sealed class FreeDeliveryDiscount : IDiscount
{
    private readonly decimal _threshold;

    public string Name { get; }
    public DiscountStage Stage => DiscountStage.Delivery;

    public FreeDeliveryDiscount(string name, decimal threshold)
    {
        if (threshold < 0) throw new ArgumentOutOfRangeException(nameof(threshold));
        Name = name;
        _threshold = threshold;
    }

    public bool Apply(DiscountContext ctx)
    {
        // Строгое условие: сумма товаров СТРОГО больше порога
        if (ctx.CurrentItemsTotal <= _threshold) return false;
        if (ctx.CurrentDeliveryCost <= 0) return false;

        var amount = ctx.CurrentDeliveryCost;
        ctx.ReduceDelivery(amount, new AppliedDiscount(
            Name, amount,
            $"Сумма товаров {ctx.CurrentItemsTotal} > {_threshold}: доставка бесплатна"));
        return true;
    }
}