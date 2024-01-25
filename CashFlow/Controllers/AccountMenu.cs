using CashFlow.CashFlow.Utils;

namespace CashFlow.CashFlow.Controllers;

using Models;

class AccountMenu : CommandLine
{
    public Account account { get; private set; }

    public AccountMenu(Account account)
    {
        this.account = account;

        commands = new Dictionary<string, Action<string[]>>
        {
            { "help", HelpCommand },
            { "exit", HelpCommand },
            { "balance", BalanceCommand },
            { "transactions", HelpCommand },
            { "exchange", ExchangeCommand }
            
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
    
    private void ExchangeCommand(string[] args)
    {
        new CurrencyMenu().Run();
    }

    private void BalanceCommand(string[] args)
    {
        Logger.Info($"Account balance: {account.AccountBalance} PLN");
    }
}
