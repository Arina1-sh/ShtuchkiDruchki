using ShtuchkiDruchki.Models;
using ShtuchkiDruchki.Money;

namespace ShtuchkiDruchki.Discounts;

public sealed class FixedDiscount : IDiscount
{
    private readonly decimal _value;
    private readonly decimal _threshold;

    public string Name { get; }
    public DiscountStage Stage => DiscountStage.Fixed;

    public FixedDiscount(string name, decimal value, decimal threshold)
    {
        if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
        if (threshold < 0) throw new ArgumentOutOfRangeException(nameof(threshold));

        Name = name;
        _value = value;
        _threshold = threshold;
    }

    public bool Apply(DiscountContext ctx)
    {
        if (ctx.CurrentItemsTotal < _threshold) return false;

        var amount = Math.Min(_value, ctx.CurrentItemsTotal);
        if (amount <= 0) return false;

        ctx.ReduceItems(amount, new AppliedDiscount(
            Name, amount, $"Фиксированная скидка {_value} при сумме от {_threshold}"));
        return true;
    }
}