using ShtuchkiDruchki.Discounts;
using ShtuchkiDruchki.Money;

namespace ShtuchkiDruchki.Models;

public sealed class DiscountContext
{
    public Order Order { get; }

    /// <summary>Текущая промежуточная сумма товаров (меняется после каждой скидки).</summary>
    public decimal CurrentItemsTotal { get; private set; }

    /// <summary>Текущая стоимость доставки.</summary>
    public decimal CurrentDeliveryCost { get; private set; }

    /// <summary>Уже применённые скидки в порядке применения.</summary>
    public List<AppliedDiscount> AppliedDiscounts { get; } = new();

    /// <summary>Введённый промокод (может быть null).</summary>
    public string? PromoCode => Order.PromoCode;

    /// <summary>Применённый промокод (если был принят).</summary>
    public PromoCodeDiscount? AppliedPromoCode { get; private set; }

    public bool PromoCodeConsumed { get; private set; }

    public DiscountContext(Order order)
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
        CurrentItemsTotal = order.Items.Sum(i => i.Product.BasePrice * i.Quantity);
        CurrentDeliveryCost = order.DeliveryCost;
    }

    public decimal BaseTotal => Order.Items.Sum(i => i.Product.BasePrice * i.Quantity) + Order.DeliveryCost;

    public void ReduceItems(decimal amount, AppliedDiscount record)
    {
        if (amount <= 0) return;
        amount = Math.Min(MoneyHelper.Round(amount), CurrentItemsTotal);
        if (amount <= 0) return;
        CurrentItemsTotal = MoneyHelper.Round(CurrentItemsTotal - amount);
        AppliedDiscounts.Add(record);
    }

    public void ReduceDelivery(decimal amount, AppliedDiscount record)
    {
        if (amount <= 0) return;
        amount = Math.Min(MoneyHelper.Round(amount), CurrentDeliveryCost);
        if (amount <= 0) return;
        CurrentDeliveryCost = MoneyHelper.Round(CurrentDeliveryCost - amount);
        AppliedDiscounts.Add(record);
    }

    public void MarkPromoCodeApplied(PromoCodeDiscount promo)
    {
        AppliedPromoCode = promo;
        PromoCodeConsumed = true;
    }

    public PricingResult ToResult() =>
        new(BaseTotal, AppliedDiscounts.ToList(), CurrentItemsTotal + CurrentDeliveryCost);
}