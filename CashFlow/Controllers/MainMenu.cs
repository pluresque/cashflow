namespace CashFlow.CashFlow.Controllers;

using Models;
using Utils;

class App : Prompt
{
    private CurrencyConverter Converter = new CurrencyConverter("", "");
    
    public App()
    {
        // Initialize commands with corresponding actions
        Commands = new Dictionary<string, Action<string[]>>
        {
            { "help", HelpCommand },
            { "exit", HelpCommand },
            { "select", SelectCommand },
            { "accounts", AccountsCommand },
            { "settings", SettingsCommand },
            { "exchange", ExchangeCommand },
            { "convert",  ConvertCommand}
        };
        
        // Check if there are no accounts in the database
        if (!database.Accounts.Any())
        {
            // Log a message and prompt the user to create an account
            PrettyPrint.Info("You don't seem to have an account, please create one.");
            CreateAccountCommand();
        }
        
        // Display help information by default (only on first run)
        HelpCommand(Array.Empty<string>());
    }
    
    public override void Run()
    {
        // Run the loop indefinitely
        while (true)
        {
            // Prompt user for input
            string input = ReadInput("\n>", "");
            
            // Check if the input is empty, if so, continue to the next iteration
            if (string.IsNullOrWhiteSpace(input))
                continue;
            
            // Parse the input, if parsing fails, exit the loop
            if (!ParseInput(input))
                break;
        }
    }
    
    private void SettingsCommand(string[] args)
    {
        // Display available settings information
        PrettyPrint.Info(
            "Available settings:\n"
            + "1. transactionsPerPage: int (min: 10, max: 100, default: 30): changes trans. per one page\n"
            + "2. preferredCurrency: string (PLN, USD, EUR, default: PLN): change default currency\n"
        );

        // Prompt the user to input the setting number or name
        string input = ReadInput("? What setting should be changed (type number or name):", "");

        // Process the user's input for the setting
        switch (input.ToLower())
        {
            case "1":
            case "transactionsperpage":
                UpdateTransactionsPerPage();
                break;
            case "2":
            case "preferredcurrency":
                UpdatePreferredCurrency();
                break;
            default:
                // Display an information message for an unknown choice
                PrettyPrint.Info("Unknown choice");
                break;
        }
    }

    private void UpdateTransactionsPerPage()
    {
        // Prompt the user to input the number of transactions per page
        string number = ReadInput("? Please provide an integer value [30]:", "30");

        // Check if the input is a valid integer
        if (int.TryParse(number, out int value))
        {
            // Check if the value is within the valid range
            if (value < 10 || value > 100)
            {
                PrettyPrint.Info("Too high/low value. Minimum is 10 and maximum is 100");
                return;
            }

            // Update the transactions per page setting in the database
            database.SetTransactionsPerPage(value);
            PrettyPrint.Success($"Successfully changed it to {value}");
        }
        else
        {
            // Display an information message for an invalid input
            PrettyPrint.Info("Invalid input. Please enter a valid integer.");
        }
    }

    private void UpdatePreferredCurrency()
    {
        // Prompt the user to input the preferred currency
        string currency = ReadInput("? Please provide preferred currency [PLN]:", "PLN");

        // Check if the provided currency is valid
        if (CheckCurrency.IsValidCurrency(currency))
        {
            // Update the preferred currency setting in the database
            database.SetPreferredCurrency(currency);
            PrettyPrint.Success($"Successfully changed it to {currency}");
        }
        else
        {
            // Display an information message for an unsupported currency
            PrettyPrint.Info("This currency is not supported");
        }
    }
    
    private void ExchangeCommand(string[] args)
    {
        PrettyPrint.Info(
            $"Current exchange rates:\n"
            + $"- 1 USD to PLN {Converter.OfflineConvertCurrency(1, "USD", "PLN")}\n"
            + $"- 1 EUR to PLN {Converter.OfflineConvertCurrency(1, "EUR", "PLN")}"
        );
    }

    private void ConvertCommand(string[] args)
    {
        // Display available currencies
        PrettyPrint.Info("Available currencies: PLN, USD, EUR:\n");

        // Prompt the user to input the amount to be converted
        string input = ReadInput("? What amount should be converted:", "");

        // Check if the provided input is a valid integer
        if (!int.TryParse(input, out int value))
        {
            PrettyPrint.Info("Not an integer");
            return;
        }

        // Prompt the user to input the source currency
        string fromCurrency = ReadInput("? Please provide the currency to convert from:", "");

        // Prompt the user to input the target currency
        string toCurrency = ReadInput("? Please provide the currency to convert to:", "");

        // Check if the provided currencies are valid
        if (!CheckCurrency.IsValidCurrency(toCurrency) || !CheckCurrency.IsValidCurrency(fromCurrency))
        {
            PrettyPrint.Info("One or both of the currencies are not supported");
            return;
        }

        // Perform the currency conversion and display the result
        PrettyPrint.Info($"{value} {fromCurrency} to {toCurrency} = " +
                       $"{Converter.OfflineConvertCurrency(value, fromCurrency, toCurrency)}");
    }

    private void AccountsCommand(string[] args)
    {
        switch (args.Length)
        {
            // Check if there are too many arguments
            case >= 2:
                PrettyPrint.Info("Too many arguments");
                break;
            // Check if no arguments are provided, show all accounts then
            case 0:
                AvailableAccounts();
                break;
            default:
                // Process the provided argument
                switch (args[0])
                {
                    case "remove":
                        RemoveAccountCommand();
                        break;
                    case "create":
                        CreateAccountCommand();
                        break;
                    default:
                        // Display an error message for an invalid command
                        PrettyPrint.Info("Invalid command. Type 'help' for a list of commands.");
                        break;
                }

                break;
        }
    }

    private void SelectCommand(string[] args)
    {
        // Retrieve the list of accounts from the database
        var accounts = database.Accounts;

        // Check if there are no accounts to select
        if (database.AccountsEmpty())
        {
            PrettyPrint.Info("There is nothing to select. Please create an account using `account create`");
            return;
        }

        // Prompt the user to input the account ID or name
        string input = ReadInput(
            "? What account should be selected (type account id or name)",
            ""
        );

        string accountName;
        
        // Check if the input is a valid integer (account ID)
        if (int.TryParse(input, out int index))
        {
            // Check if the index is within the valid range
            if (index > accounts.Count || index <= 0)
            {
                PrettyPrint.Info("Account does not exist");
                return;
            }
            
            // Get the account name based on the selected index
            accountName = accounts[index - 1].AccountName;
        }
        else
        {
            // Check if the account with the provided name exists
            if (!database.AccountExist(input))
            {
                PrettyPrint.Info("Account does not exist");
                return;
            }
            
            // Use the provided name as the account name
            accountName = input;
        }
        
        // Display a success message with the selected account name
        PrettyPrint.Success($"You've chosen an account called {accountName}");
        
        // Run the menu for the selected account
        new AccountMenu(database.GetAccount(accountName)).Run();
    }

    private void CreateAccountCommand()
    {
        // Generate a funny name for the account
        string generatedName = NameGenerator.GenerateFunnyName();

        // Prompt the user to input the account name, providing the generated name as a default
        string name = ReadInput(
            $"? How should we call this account [{generatedName}]:",
            generatedName
        );

        // Attempt to add the new account to the database
        if (!database.AddAccount(new Account(name, 0)))
        {
            PrettyPrint.Info("Account already exists.");
        }
        else
        {
            PrettyPrint.Success("Successfully created an account.");
        }
    }

    private void RemoveAccountCommand()
    {
        // Retrieve the list of accounts from the database
        var accounts = database.Accounts;

        // Check if there are no accounts to delete
        if (database.AccountsEmpty())
        {
            PrettyPrint.Info("There is nothing to delete");
            return;
        }

        // Prompt the user to input the account ID or name
        string input = ReadInput($"? What account should be deleted (type account id or name)", "");
        string accountName;

        // Check if the input is a valid integer (account ID)
        if (int.TryParse(input, out int index))
        {
            // Check if the index is within the valid range
            if (index > accounts.Count || index == 0)
            {
                PrettyPrint.Info("Account does not exist");
                return;
            }

            // Get the account name based on the selected index
            accountName = accounts[index - 1].AccountName;
        }
        else
        {
            // Check if the account with the provided name exists
            if (!database.AccountExist(input))
            {
                PrettyPrint.Info("Account does not exist");
                return;
            }

            // Use the provided name as the account name
            accountName = input;
        }

        // Remove the selected account from the database
        database.RemoveAccount(accountName);
        PrettyPrint.Success($"Successfully deleted the account {accountName}");
    }

    private void AvailableAccounts()
    {
        // Retrieve the list of accounts from the database
        var accounts = database.Accounts;

        // Display a header for available accounts
        PrettyPrint.Info("Available accounts:");

        // Check if there are no accounts to show
        if (database.AccountsEmpty())
        {
            PrettyPrint.Info("There is nothing to show. Please create an account using `account create`");
            return;
        }

        // Display each account with its index, name, and balance
        for (int index = 0; index < accounts.Count; index++)
        {
            Account account = accounts[index];
            Console.WriteLine($"{index + 1}. {account.AccountName} - {account.AccountBalance} " +
                              $"{database.PreferredCurrency} ");
        }

        // Add an empty line for better readability
        Console.WriteLine();

        // Display available options for the user
        PrettyPrint.Info("Available options:\n" + "- accounts create\n- accounts remove");
    }
}
