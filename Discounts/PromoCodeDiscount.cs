using ShtuchkiDruchki.Models;
using ShtuchkiDruchki.Money;

namespace ShtuchkiDruchki.Discounts;

public sealed class PromoCodeDiscount : IDiscount
{
    public string Code { get; }
    public bool IsPercent { get; }
    public decimal Value { get; }
    public DateTime? ValidUntil { get; }
    public bool IsActive { get; }
    public string Name { get; }

    /// <summary>Фиксированный промокод — стадия Fixed; процентный — стадия Percent.</summary>
    public DiscountStage Stage => IsPercent ? DiscountStage.Percent : DiscountStage.Fixed;

    public PromoCodeDiscount(string name, string code, bool isPercent,
                             decimal value, DateTime? validUntil, bool isActive)
    {
        Name = name;
        Code = code ?? throw new ArgumentNullException(nameof(code));
        IsPercent = isPercent;
        Value = value;
        ValidUntil = validUntil;
        IsActive = isActive;
    }

    /// <summary>Подходит ли промокод к заказу (без учёта суммы).</summary>
    public bool Matches(string? promoCode, DateTime createdAt)
    {
        if (!IsActive) return false;
        if (string.IsNullOrWhiteSpace(promoCode)) return false;
        if (!string.Equals(promoCode.Trim(), Code.Trim(), StringComparison.OrdinalIgnoreCase))
            return false;
        if (ValidUntil.HasValue && createdAt.Date > ValidUntil.Value.Date)
            return false;
        return true;
    }

    public bool Apply(DiscountContext ctx)
    {
        if (ctx.PromoCodeConsumed) return false;
        if (!Matches(ctx.PromoCode, ctx.Order.CreatedAt)) return false;

        decimal amount;
        if (IsPercent)
        {
            amount = MoneyHelper.Round(ctx.CurrentItemsTotal * Value / 100m);
            if (amount <= 0) return false;
            ctx.ReduceItems(amount, new AppliedDiscount(
                Name, amount, $"Промокод {Code}: {Value}%"));
        }
        else
        {
            amount = Math.Min(Value, ctx.CurrentItemsTotal);
            if (amount <= 0) return false;
            ctx.ReduceItems(amount, new AppliedDiscount(
                Name, amount, $"Промокод {Code}: {Value} руб."));
        }

        ctx.MarkPromoCodeApplied(this);
        return true;
    }
}