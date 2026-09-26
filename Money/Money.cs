namespace ShtuchkiDruchki.Money;

public static class MoneyHelper
{
    public static decimal Round(decimal value) =>
        Math.Round(value, 2, MidpointRounding.ToEven);
}