using CashFlow.CashFlow.Utils;

namespace CashFlow.CashFlow.Controllers;

using Models;

class SettingsMenu : CommandLine
{
    public SettingsMenu()
    {
        commands = new Dictionary<string, Action<string[]>>
        {
            { "help", HelpCommand },
            { "exit", HelpCommand },
            { "set", SetCommand },
            { "show", ShowCommand },
        };
    }

    public override void Run()
    {
        while (true)
        {
            string input = ReadInput($"\nSETTINGS >", "");
            if (input == "")
                continue;

            if (!ParseInput(input))
                break;
        }
    }

    private void ShowCommand(string[] args)
    {
        Logger.Info(
            $"Current settings\n"
                + $"1. transactionsPerPage: {database.transactionsPerPage}\n"
                + $"2. preferredCurrency: {database.preferredCurrency}"
        );
    }

    private void SetCommand(string[] args)
    {
        Logger.Info(
            "Available settings:\n"
                + "1. transactionsPerPage: int (min: 10, max: 100, default: 30): changes trans. per one page\n"
                + "2. preferredCurrency: string (PLN, USD, EUR, default: PLN): change default currency\n"
        );

        string input = ReadInput($"? What setting should be changed (type number or name):", "");

        switch (input)
        {
            case "1"
            or "transactionsPerPage":
                string number = ReadInput("? Please provide integer value [30]: ", "30");

                if (int.TryParse(number, out int value))
                {
                    if (value is > 100 or < 10)
                    {
                        Logger.Info("Too high/low value. Minimum is 10 and maximum is 100");
                        return;
                    }

                    database.SetTransactionsPerPage(value);
                }
                Logger.Success($"Successfully changed it to {value}");
                break;
            case "2"
            or "preferredCurrency":
                string currency = ReadInput("? Please provide preferred currency [PLN]:", "PLN");
                if (currency is not ("PLN" or "USD" or "EUR"))
                {
                    Logger.Info("This currency is not support");
                    return;
                }

                database.SetPreferredCurrency(currency);
                Logger.Success($"Successfully changed it to {currency}");
                break;
            default:
                Logger.Info("Unknown choice");
                break;
        }
    }
}
