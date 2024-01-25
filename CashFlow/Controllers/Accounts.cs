namespace CashFlow.CashFlow.Controllers;

using Models;


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