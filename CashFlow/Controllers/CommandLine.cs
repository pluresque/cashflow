using CashFlow.CashFlow.Utils;
using CashFlow.CashFlow.Models;

namespace CashFlow.CashFlow.Controllers;


abstract class CommandLine
{
    protected Dictionary<string, Action<string[]>> commands = null!;
    protected readonly Database database = new();
    
    public abstract void Run();

    protected static string ReadInput(string prompt, string defaultValue)
    {
        Console.Write(prompt + " ");
        var answer = Console.ReadLine();
        return string.IsNullOrWhiteSpace(answer) ? defaultValue : answer.Trim();
    }

    protected bool ParseInput(string input)
    {
        string[] inputParts = input.Split(' ');
        string command = inputParts[0].ToLower();
        
        if (command == "exit")
            return false;  // Meaning exit

        if (!commands.ContainsKey(command)) 
            Logger.Info("Invalid command. Type 'help' for a list of commands.");
        else
            commands[command](inputParts.Length > 1 ? inputParts[1..] : Array.Empty<string>());
        
        return true; // Continue
    }

    protected void HelpCommand(string[] args)
    {
        Logger.Info("Available Commands:");
        foreach (var cmd in commands.Keys)
        {
            Console.WriteLine($"- {cmd}");
        }
    }
}


class AccountMenu : CommandLine
{
    private string account;

    public AccountMenu(string account)
    {
        this.account = account;
        
        commands = new Dictionary<string, Action<string[]>>
        {
            { "help", HelpCommand },
            { "exit", HelpCommand }, // Just a placeholder,
            { "balance", HelpCommand },
            { "transactions", HelpCommand },
            { "transadd", HelpCommand },
            { "transremove", HelpCommand}
        };
    }
    
    public override void Run()
    {
        while (true)
        {
            string input = ReadInput($"\n{account} >", "");
            if (input == "") continue;

            if (!ParseInput(input))
                break;
        }
    }
}

class App : CommandLine
{
    public App()
    {
        commands = new Dictionary<string, Action<string[]>>
        {
            { "help", HelpCommand },
            { "exit", HelpCommand },
            { "select", SelectCommand },
            { "accounts", AccountsCommand },
        };
        
        if (database.GetKey("accounts") is null)
        {
            Logger.Info("You don't seem to have an account, please create one.");
            CreateAccountCommand();
        }
        
        Console.WriteLine();
        Logger.Info("For command list you can use `help`.\n" +
                    "To list available accounts - `accounts`.\nSelecting an account - `select`");
    }
    
    public override void Run()
    {
        while (true)
        {
            string input = ReadInput("\n>", "");
            if (input == "") continue;

            if (!ParseInput(input))
                break;
        }
    }

    private void AccountsCommand(string[] args)
    {

        if (args.Length >= 2)
        {
            Logger.Info("Too many arguments");
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
                Logger.Info("Invalid command. Type 'help' for a list of commands.");
                return;
        } 
        
    }
    
    private void SelectCommand(string[] args)
    {
        var accounts = database.GetKey("accounts");
        if (accounts is null || accounts.Count == 0)
        {
            Logger.Info("There is nothing to select. Please create an account using `account create`");
            return;
        }
        
        string input = ReadInput($"? What account should be selected (type account id or name)", "");
        string accountName;
        
        if (int.TryParse(input, out int index))
        {
            if (index > accounts.Count || index <= 0)
            {
                Logger.Info("Account does not exist");
                return;
            }
            
            accountName = accounts.Keys.ToList()[index - 1];
        }
        else
        {
            if (!accounts.ContainsKey(input))
            {
                Logger.Info("Account does not exist");
                return; 
            }

            accountName = input;
        }
        
        Logger.Success($"You've chosen an account called {accountName}");
        new AccountMenu(accountName).Run();
    }
    
    private void CreateAccountCommand()
    {
        string GeneratedName = NamesGenerator.GenerateFunnyName();
        string name = ReadInput($"? How should we call this account [{GeneratedName}]:", GeneratedName);
        
        var accounts = database.GetAccounts();
        
        if (accounts is null || accounts.Count == 0)
        {
            database.SetKey("accounts", new Dictionary<string, object> { { name, 0 } });
        } else if (accounts.ContainsKey(name))
        {
            Logger.Info("This account already exists.");
            return;
        }
        else
        {
            accounts.Add(name, 0);
            database.SetKey("accounts", accounts);
        }
        
        Logger.Success("Successfully created an account.");
    }

    private void RemoveAccountCommand()
    {
        var accounts = database.GetAccounts();
        
        if (accounts is null || accounts.Count <= 0)
        {
            Logger.Info("There is nothing to delete");
            return;
        }
        
        string input = ReadInput($"? What account should be deleted (type account id or name)", "");
        string accountName;
        
        
        if (int.TryParse(input, out int index))
        {
            if (index > accounts.Count || index <= 0)
            {
                Logger.Info("Account does not exist");
                return;
            }
            
            accountName = accounts.Keys.ToList()[index - 1];
        }
        else
        {
            if (!accounts.ContainsKey(input))
            {
                Logger.Info("Account does not exist");
                return; 
            }

            accountName = input;
        }
        
        accounts.Remove(accountName);
        database.SetKey("accounts", accounts);
                
        Logger.Success($"Successfully deleted the account {accountName}");
    }

    private void AvailableAccounts()
    {
        var accounts = database.GetAccounts();
        Logger.Info("Available accounts:");
        
        if (accounts is null || accounts.Count == 0)
        {
            Logger.Info("There is nothing to show. Please create an account using `account create`");
            return;
        }
        
        foreach (string key in accounts!.Keys)
        {
            var value = accounts[key];
            var index = accounts.Keys.ToList().IndexOf(key) + 1;
            Console.WriteLine($"{index}. {key}: {value} PLN");
        }
        
        Console.WriteLine();
        
        Logger.Info("Available options:\n" +
                    "- accounts create\n- accounts remove");
    }
    
}