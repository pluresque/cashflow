namespace CashFlow.CashFlow.Utils;

public static class CheckCurrency
{
    public static bool IsValidCurrency(string currency)
    {
        return currency.Equals("PLN", StringComparison.OrdinalIgnoreCase)
               || currency.Equals("USD", StringComparison.OrdinalIgnoreCase)
               || currency.Equals("EUR", StringComparison.OrdinalIgnoreCase);
    }
}