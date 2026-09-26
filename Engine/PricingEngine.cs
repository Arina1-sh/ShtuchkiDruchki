using ShtuchkiDruchki.Discounts;
using ShtuchkiDruchki.Money;
using ShtuchkiDruchki.Models;

namespace ShtuchkiDruchki.Engine;

public sealed class PricingEngine
{
    private readonly IReadOnlyList<IDiscount> _discounts;
    private readonly IReadOnlyList<PromoCodeDiscount> _promoCodes;

    public PricingEngine(IReadOnlyList<IDiscount> discounts,
                         IReadOnlyList<PromoCodeDiscount>? promoCodes = null)
    {
        _discounts = discounts ?? throw new ArgumentNullException(nameof(discounts));
        _promoCodes = promoCodes ?? Array.Empty<PromoCodeDiscount>();
    }

    public PricingResult Calculate(Order order)
    {
        if (order is null) throw new ArgumentNullException(nameof(order));
        var ctx = new DiscountContext(order);

        // Стадия 1: Quantity — «3 по цене 2»
        RunStage(ctx, DiscountStage.Quantity, _discounts);

        // Стадия 2: Percent — процентные + выбор между обычной и промокодом
        RunPercentStage(ctx);

        // Стадия 3: Fixed — обычные фиксированные + фиксированный промокод
        RunStage(ctx, DiscountStage.Fixed, _discounts);
        foreach (var promo in _promoCodes)
            if (!promo.IsPercent)
                promo.Apply(ctx);

        // Стадия 4: Delivery — бесплатная доставка
        RunStage(ctx, DiscountStage.Delivery, _discounts);

        return ctx.ToResult();
    }

    private static void RunStage(DiscountContext ctx, DiscountStage stage,
                                 IReadOnlyList<IDiscount> discounts)
    {
        foreach (var d in discounts)
            if (d.Stage == stage)
                d.Apply(ctx);
    }

    private void RunPercentStage(DiscountContext ctx)
    {
        // Обычные процентные скидки (кроме Loyalty и FirstOrder — они отдельные)
        var regularPercents = _discounts
            .Where(d => d.Stage == DiscountStage.Percent
                        && d is not LoyaltyDiscount
                        && d is not FirstOrderDiscount)
            .ToList();

        // Процентные промокоды, подходящие к заказу
        var percentPromos = _promoCodes
            .Where(p => p.IsPercent && p.Matches(ctx.PromoCode, ctx.Order.CreatedAt))
            .ToList();

        // Считаем выгоду обычной процентной и процентного промокода
        var bestRegular = ComputeBestPercentAmount(ctx, regularPercents);
        var bestPromo = ComputeBestPercentAmount(
            ctx, percentPromos.Cast<IDiscount>().ToList());

        // Выбираем большую выгоду, при равенстве — промокод
        if (bestPromo.Amount > 0 && bestPromo.Amount >= bestRegular.Amount)
            bestPromo.Discount!.Apply(ctx);
        else if (bestRegular.Amount > 0)
            bestRegular.Discount!.Apply(ctx);

        // Loyalty и FirstOrder — отдельные процентные, применяются после победителя
        foreach (var d in _discounts)
        {
            if (d.Stage != DiscountStage.Percent) continue;
            if (d is LoyaltyDiscount || d is FirstOrderDiscount)
                d.Apply(ctx);
        }
    }

    private static (IDiscount? Discount, decimal Amount) ComputeBestPercentAmount(
        DiscountContext ctx, IReadOnlyList<IDiscount> percents)
    {
        IDiscount? best = null;
        decimal bestAmount = 0m;

        foreach (var d in percents)
        {
            decimal amount = d switch
            {
                PercentDiscount p => MoneyHelper.Round(ctx.CurrentItemsTotal * p.Percent / 100m),
                PromoCodeDiscount promo => MoneyHelper.Round(ctx.CurrentItemsTotal * promo.Value / 100m),
                _ => 0m
            };

            if (amount > bestAmount)
            {
                bestAmount = amount;
                best = d;
            }
        }

        return (best, bestAmount);
    }
}