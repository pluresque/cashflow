using CashFlow.CashFlow.Utils;

namespace CashFlow.CashFlow.Models;

public class CurrencyConverter
{
    private readonly string apiKey;
    private readonly string apiUrl;

    public CurrencyConverter(string apiKey, string apiUrl)
    {
        this.apiKey = apiKey;
        this.apiUrl = apiUrl;
    }

    private async Task<double> ConvertCurrency(
        decimal amount,
        string fromCurrency,
        string toCurrency
    )
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
