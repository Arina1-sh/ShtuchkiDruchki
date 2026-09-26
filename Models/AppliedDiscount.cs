using ShtuchkiDruchki.Money;

namespace ShtuchkiDruchki.Models;

public sealed class AppliedDiscount
{
    public string Name { get; }
    public decimal Amount { get; }
    public string Reason { get; }

    public AppliedDiscount(string name, decimal amount, string reason)
    {
        Name = name;
        Amount = MoneyHelper.Round(amount);
        Reason = reason;
    }
}