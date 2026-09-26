using ShtuchkiDruchki.Money;

namespace ShtuchkiDruchki.Models;

public sealed class PricingResult
{
    public decimal BaseTotal { get; }
    public IReadOnlyList<AppliedDiscount> AppliedDiscounts { get; }
    public decimal FinalTotal { get; }

    public PricingResult(decimal baseTotal, IReadOnlyList<AppliedDiscount> appliedDiscounts, decimal finalTotal)
    {
        BaseTotal = MoneyHelper.Round(baseTotal);
        AppliedDiscounts = appliedDiscounts;
        FinalTotal = MoneyHelper.Round(finalTotal);
    }
}