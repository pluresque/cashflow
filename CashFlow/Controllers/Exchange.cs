using CashFlow.CashFlow.Utils;

namespace CashFlow.CashFlow.Controllers;

using Models;

class Exchange : Prompt
{
    private CurrencyConverter converter = new CurrencyConverter("", "");
    
    public Exchange()
    {
        commands = new Dictionary<string, Action<string[]>>
        {
            { "help", HelpCommand },
            { "exit", HelpCommand },
            { "convert", ConvertCommand },
            { "show", ShowCommand },
        };
        
        Console.WriteLine("---------");
        HelpCommand(Array.Empty<string>());
    }

    public override void Run()
    {
        while (true)
        {
            string input = ReadInput($"\nEXCHANGE >", "");
            if (input == "")
                continue;

            if (!ParseInput(input))
                break;
        }
    }

    private void ShowCommand(string[] args)
    {
        AppLogger.Info(
            $"Current exchange rates\n"
                + $"- 1 USD to PLN {converter.OfflineConvertCurrency(1, "USD", "PLN")}\n"
                + $"- 1 EUR to PLN {converter.OfflineConvertCurrency(1, "EUR", "PLN")}"
        );
    }

    private void ConvertCommand(string[] args)
    {
        AppLogger.Info("Available currencies: PLN, USD, EUR:\n");

        string input = ReadInput($"? What amount should be converted:", "");
        
        if (!int.TryParse(input, out int value))
        {
            AppLogger.Info("Not an integer");
            return;
        }
        
        string fromCurrency = ReadInput("? Please provide currency that you would convert from:", "");
        if (fromCurrency is not ("PLN" or "USD" or "EUR"))
        {
            AppLogger.Info("This currency is not support");
            return;
        }
        
        string toCurrency = ReadInput("? Please provide currency that you would convert from:", "");
        if (toCurrency is not ("PLN" or "USD" or "EUR"))
        {
            AppLogger.Info("This currency is not support");
            return;
        }

        if (toCurrency == fromCurrency)
        {
            AppLogger.Info("Currencies cannot be the same");
            return;
        }
        
        AppLogger.Info($"{value} {fromCurrency} to {toCurrency} = " +
                    $"{converter.OfflineConvertCurrency(value, fromCurrency, toCurrency)}");
        
    }
}