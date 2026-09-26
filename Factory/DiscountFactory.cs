using System.Globalization;
using ShtuchkiDruchki.Discounts;

namespace ShtuchkiDruchki.Factory;

public static class DiscountFactory
{
    /// <summary>Создаёт стратегию по данным конфигурации.</summary>
    public static IDiscount Create(string type, IReadOnlyDictionary<string, object?> config)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("type обязателен", nameof(type));
        if (config is null) throw new ArgumentNullException(nameof(config));

        string name = GetString(config, "name") ?? type;

        return type.ToLowerInvariant() switch
        {
            "percent" => new PercentDiscount(name, GetDecimal(config, "value")),
            "fixed" => new FixedDiscount(name, GetDecimal(config, "value"), GetDecimal(config, "threshold")),
            "threefortwo" => new ThreeForTwoDiscount(name,
                                GetString(config, "category")
                                ?? throw new ArgumentException("threeForTwo: category обязательна")),
            "loyalty" => new LoyaltyDiscount(name, GetDecimal(config, "percent"), GetInt(config, "days")),
            "freedelivery" => new FreeDeliveryDiscount(name, GetDecimal(config, "threshold")),
            "firstorder" => new FirstOrderDiscount(name, GetDecimal(config, "percent")),
            "promo" => new PromoCodeDiscount(
                                name,
                                GetString(config, "code")
                                ?? throw new ArgumentException("promo: code обязателен"),
                                GetBool(config, "isPercent"),
                                GetDecimal(config, "value"),
                                GetDate(config, "validUntil"),
                                GetBool(config, "isActive")),
            _ => throw new ArgumentException($"Неизвестный тип скидки: {type}")
        };
    }

    private static string? GetString(IReadOnlyDictionary<string, object?> c, string k)
        => c.TryGetValue(k, out var v) ? v?.ToString() : null;

    private static decimal GetDecimal(IReadOnlyDictionary<string, object?> c, string k)
    {
        if (!c.TryGetValue(k, out var v) || v is null)
            throw new ArgumentException($"Поле '{k}' обязательно");
        return Convert.ToDecimal(v, CultureInfo.InvariantCulture);
    }

    private static int GetInt(IReadOnlyDictionary<string, object?> c, string k)
    {
        if (!c.TryGetValue(k, out var v) || v is null)
            throw new ArgumentException($"Поле '{k}' обязательно");
        return Convert.ToInt32(v, CultureInfo.InvariantCulture);
    }

    private static bool GetBool(IReadOnlyDictionary<string, object?> c, string k)
        => c.TryGetValue(k, out var v) && v is not null && Convert.ToBoolean(v);

    private static DateTime? GetDate(IReadOnlyDictionary<string, object?> c, string k)
    {
        if (!c.TryGetValue(k, out var v) || v is null) return null;
        if (v is DateTime dt) return dt;
        return DateTime.Parse(v.ToString()!, CultureInfo.InvariantCulture);
    }
}