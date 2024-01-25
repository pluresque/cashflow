namespace CashFlow.CashFlow.Controllers;

using Models;
using Utils;

class App : Prompt
{
    public App()
    {
        commands = new Dictionary<string, Action<string[]>>
        {
            { "help", HelpCommand },
            { "exit", HelpCommand },
            { "select", SelectCommand },
            { "accounts", AccountsCommand },
            { "settings", SettingsCommand },
            { "exchange", ExchangeCommand }
        };

        if (!database.Accounts.Any())
        {
            AppLogger.Info("You don't seem to have an account, please create one.");
            CreateAccountCommand();
        }

        Console.WriteLine();
        AppLogger.Info(
            "For command list you can use `help`.\n"
                + "To list available accounts - `accounts`.\nSelecting an account - `select`"
        );
    }

    public override void Run()
    {
        while (true)
        {
            string input = ReadInput("\n>", "");
            if (input == "")
                continue;

            if (!ParseInput(input))
                break;
        }
    }

    private void ExchangeCommand(string[] args)
    {
        new Exchange().Run();
    }

    private void SettingsCommand(string[] args)
    {
        new Settings().Run();
    }

    private void AccountsCommand(string[] args)
    {
        if (args.Length >= 2)
        {
            AppLogger.Info("Too many arguments");
            return;
        }

        if (args.Length == 0)
        {
            AvailableAccounts();
            return;
        }

        switch (args[0])
        {
            case "remove":
                RemoveAccountCommand();
                return;
            case "create":
                CreateAccountCommand();
                return;
            default:
                AppLogger.Info("Invalid command. Type 'help' for a list of commands.");
                return;
        }
    }

    private void SelectCommand(string[] args)
    {
        var accounts = database.Accounts;
        if (accounts.Count == 0)
        {
            AppLogger.Info(
                "There is nothing to select. Please create an account using `account create`"
            );
            return;
        }

        string input = ReadInput(
            $"? What account should be selected (type account id or name)",
            ""
        );
        string accountName;

        if (int.TryParse(input, out int index))
        {
            if (index > accounts.Count || index <= 0)
            {
                AppLogger.Info("Account does not exist");
                return;
            }

            accountName = accounts[index - 1].Name;
        }
        else
        {
            if (!database.AccountExist(input))
            {
                AppLogger.Info("Account does not exist");
                return;
            }

            accountName = input;
        }

        AppLogger.Success($"You've chosen an account called {accountName}");
        new Account(database.GetAccount(accountName)).Run();
    }

    private void CreateAccountCommand()
    {
        string GeneratedName = Utils.NameGenerator.GenerateFunnyName();
        string name = ReadInput(
            $"? How should we call this account [{GeneratedName}]:",
            GeneratedName
        );

        if (!database.AddAccount(new Models.Account(name, 0)))
            AppLogger.Info("Account already exists.");
        else
            AppLogger.Success("Successfully created an account.");
    }

    private void RemoveAccountCommand()
    {
        var accounts = database.Accounts;

        if (accounts.Count == 0)
        {
            AppLogger.Info("There is nothing to delete");
            return;
        }

        string input = ReadInput($"? What account should be deleted (type account id or name)", "");
        string accountName;

        if (int.TryParse(input, out int index))
        {
            if (index > accounts.Count || index == 0)
            {
                AppLogger.Info("Account does not exist");
                return;
            }

            accountName = accounts[index - 1].Name;
        }
        else
        {
            if (!database.AccountExist(input))
            {
                AppLogger.Info("Account does not exist");
                return;
            }

            accountName = input;
        }

        database.RemoveAccount(accountName);
        AppLogger.Success($"Successfully deleted the account {accountName}");
    }

    private void AvailableAccounts()
    {
        var accounts = database.Accounts;
        AppLogger.Info("Available accounts:");

        if (accounts.Count == 0)
        {
            AppLogger.Info(
                "There is nothing to show. Please create an account using `account create`"
            );
            return;
        }

        for (int index = 0; index < accounts.Count; index++)
        {
            Models.Account account = accounts[index];
            Console.WriteLine(
                $"{index + 1}. {account.AccountName} - {account.AccountBalance} PLN "
            );
        }

        Console.WriteLine();

        AppLogger.Info("Available options:\n" + "- accounts create\n- accounts remove");
    }
}
