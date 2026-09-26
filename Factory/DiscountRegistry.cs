using ShtuchkiDruchki.Discounts;

namespace ShtuchkiDruchki.Factory;

public sealed class DiscountRegistry
{
    private static readonly Lazy<DiscountRegistry> _instance =
        new(() => new DiscountRegistry(), LazyThreadSafetyMode.ExecutionAndPublication);

    public static DiscountRegistry Instance => _instance.Value;

    private readonly List<IDiscount> _regularDiscounts = new();
    private readonly List<PromoCodeDiscount> _promoCodes = new();

    private DiscountRegistry() { }

    /// <summary>Загружает список обычных скидок (без промокодов).</summary>
    public void LoadRegularDiscounts(IEnumerable<IReadOnlyDictionary<string, object?>> configs)
    {
        _regularDiscounts.Clear();
        foreach (var cfg in configs)
        {
            var type = cfg.TryGetValue("type", out var t) ? t?.ToString() : null;
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("В конфигурации отсутствует поле 'type'");

            var d = DiscountFactory.Create(type!, cfg);
            if (d is PromoCodeDiscount promo)
                _promoCodes.Add(promo);
            else
                _regularDiscounts.Add(d);
        }
    }

    /// <summary>Загружает промокоды отдельным списком.</summary>
    public void LoadPromoCodes(IEnumerable<IReadOnlyDictionary<string, object?>> configs)
    {
        _promoCodes.Clear();
        foreach (var cfg in configs)
        {
            var d = DiscountFactory.Create("promo", cfg);
            _promoCodes.Add((PromoCodeDiscount)d);
        }
    }

    public IReadOnlyList<IDiscount> GetRegularDiscounts() => _regularDiscounts.AsReadOnly();
    public IReadOnlyList<PromoCodeDiscount> GetPromoCodes() => _promoCodes.AsReadOnly();

    /// <summary>Сбрасывает реестр (для тестов).</summary>
    public void Reset()
    {
        _regularDiscounts.Clear();
        _promoCodes.Clear();
    }
}