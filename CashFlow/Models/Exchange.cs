using CashFlow.CashFlow.Utils;

namespace CashFlow.CashFlow.Models;

public class CurrencyConverter
{
    private readonly string apiKey;
    private readonly string apiUrl;
    
    // Offline exchange rates
    private const double UsdToPlnRate = 4.0;
    private const double EurToPlnRate = 4.5;

    public CurrencyConverter(string apiKey, string apiUrl)
    {
        this.apiKey = apiKey;
        this.apiUrl = apiUrl;
    }

    public double OfflineConvertCurrency(double amount, string fromCurrency, string toCurrency)
    {
        // Convert input amount to PLN (Polish Zloty)

        double plnAmount = fromCurrency.ToUpper() switch
        {
            "PLN" => amount,
            "USD" => amount * UsdToPlnRate,
            "EUR" => amount * EurToPlnRate,
            _ => throw new ArgumentException("Unsupported source currency.")
        };

        // Convert PLN amount to the target currency

        double result = toCurrency.ToUpper() switch
        {
            "PLN" => plnAmount,
            "USD" => plnAmount / UsdToPlnRate,
            "EUR" => plnAmount / EurToPlnRate,
            _ => throw new ArgumentException("Unsupported target currency.")
        };

        return result;
    }
    
    public async Task<double> ConvertCurrency(decimal amount, string fromCurrency, string toCurrency)
    {
        using HttpClient client = new HttpClient();

        // Only supports https://exchangeratesapi.io/ as of now
        string url = $"{apiUrl}?apiKey={apiKey}&from={fromCurrency}&to={toCurrency}";
        HttpResponseMessage response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string result = await response.Content.ReadAsStringAsync();
            decimal exchangeRate = decimal.Parse(result);
            return (double)(amount * exchangeRate);
        }

        throw new ApiException($"{response.StatusCode} - {response.ReasonPhrase}");
    }
}
