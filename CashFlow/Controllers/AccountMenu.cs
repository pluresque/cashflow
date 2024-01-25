using CashFlow.CashFlow.Utils;

namespace CashFlow.CashFlow.Controllers;

using Models;

class AccountMenu : Prompt
{
    public Account account { get; private set; }

    public AccountMenu(Models.Account account)
    {
        this.account = account;

        commands = new Dictionary<string, Action<string[]>>
        {
            { "help", HelpCommand },
            { "exit", HelpCommand },
            { "balance", BalanceCommand },
            { "transactions", TransactionCommand },
            { "exchange", ExchangeCommand },
            { "topup", TopUpCommand}
            
        };
    }

    public override void Run()
    {
        while (true)
        {
            string input = ReadInput($"\n{account.AccountName} >", "");
            if (input == "")
                continue;

            if (!ParseInput(input))
                break;
        }
    }

    private void TransactionCommand(string[] args)
    {
        AppLogger.Info("All transactions made by this account:");

        foreach (Transaction transaction in database.transactions)
        {
            if (transaction.AccountName == account.AccountName)
                Console.WriteLine($"- {transaction.Name} {transaction.Amount} {database.preferredCurrency} " +
                                  $"at {transaction.TransactionTime}");
        }
    }

    private void TopUpCommand(string[] args)
    {
        string input = ReadInput($"? What amount should be added/removed to account:", "");
        
        if (!int.TryParse(input, out int value))
        {
            AppLogger.Info("Not an integer");
            return;
        }

        double amount = account.AccountBalance + value;
        
        if (amount < 0)
        {
            AppLogger.Info("Can't be less than zero");
            return;
        }
        
        account.UpdateAccountBalance(amount);
        AppLogger.Success("Successfully updated account balance");
    }
    
    private void ExchangeCommand(string[] args)
    {
        new ExchangeMenu().Run();
    }

    private void BalanceCommand(string[] args)
    {
        AppLogger.Info($"Account balance: {account.AccountBalance} {database.preferredCurrency}");
    }
}
