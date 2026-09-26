using ShtuchkiDruchki.Discounts;
using ShtuchkiDruchki.Models;

namespace ShtuchkiDruchki.Engine;

public sealed class LoggingDiscountDecorator : IDiscount
{
    private readonly IDiscount _inner;
    private readonly Action<string> _log;

    public LoggingDiscountDecorator(IDiscount inner, Action<string>? log = null)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _log = log ?? Console.WriteLine;
    }

    public string Name => _inner.Name;
    public DiscountStage Stage => _inner.Stage;

    public bool Apply(DiscountContext ctx)
    {
        _log($"[Discount] Попытка применить '{_inner.Name}' (стадия {_inner.Stage})...");
        var applied = _inner.Apply(ctx);
        _log(applied
            ? $"[Discount] '{_inner.Name}' применена."
            : $"[Discount] '{_inner.Name}' не применена.");
        return applied;
    }
}