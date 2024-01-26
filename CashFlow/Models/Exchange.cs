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

        // Construct the API request URL
        string url = $"{apiUrl}?apiKey={apiKey}&from={fromCurrency}&to={toCurrency}";

        // Send a GET request to the exchange rates API
        HttpResponseMessage response = await client.GetAsync(url);

        // Check if the API request was successful
        if (response.IsSuccessStatusCode)
        {
            // Read the response content
            string result = await response.Content.ReadAsStringAsync();

            // Parse the exchange rate from the response
            if (decimal.TryParse(result, out decimal exchangeRate))
            {
                // Calculate and return the converted amount
                return (double)(amount * exchangeRate);
            }
            else
            {
                // Handle the case where the response content cannot be parsed
                throw new ApiException("Failed to parse the exchange rate from the API response.");
            }
        }

        // Throw an exception if the API request was not successful
        throw new ApiException($"{response.StatusCode} - {response.ReasonPhrase}");
    }

}
