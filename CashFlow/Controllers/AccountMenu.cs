using CashFlow.CashFlow.Utils;

namespace CashFlow.CashFlow.Controllers;

using Models;

class AccountMenu : Prompt
{
    public Account Account { get; private set; }

    public AccountMenu(Account account)
    {
        // Set the account field
        Account = account;
        
        // Initialize commands for the account menu
        Commands = new Dictionary<string, Action<string[]>>
        {
            { "help", HelpCommand },
            { "exit", HelpCommand },
            { "balance", BalanceCommand },
            { "transactions", TransactionCommand },
            { "topup", TopUpCommand}
            
        };
    }

    public override void Run()
    {
        // Run the loop indefinitely
        while (true)
        {
            // Prompt user for input
            string input = ReadInput($"\n{Account.AccountName} >", "");
            
            // Check if the input is empty, if so, continue to the next iteration
            if (string.IsNullOrWhiteSpace(input))
                continue;
            
            // Parse the input, if parsing fails, exit the loop
            if (!ParseInput(input))
                break;
        }
    }

    private void ShowTransactions()
    {
        // Display a header for all transactions made by this account
        PrettyPrint.Info("All transactions made by this account:");

        // Iterate through all transactions in the database
        for (int index = 0; index < database.Transactions.Count; index++)
        {
            Transaction transaction = database.Transactions[index];

            // Check if the transaction belongs to the current account
            if (transaction.AccountName == Account.AccountName)
            {
                // Display information about the transaction
                Console.WriteLine($"{index}. {transaction.Name} - {transaction.Amount} {database.PreferredCurrency} " +
                                  $"at {transaction.TransactionTime}");
            }
        }
    }

    private void TopUpCommand(string[] args)
    {
        // Prompt the user to input the amount to be added/removed
        string input = ReadInput($"? What amount should be added/removed to account:", "");

        // Check if the provided input is a valid double
        if (!double.TryParse(input, out double value))
        {
            PrettyPrint.Info("Not a valid number");
            return;
        }

        // Calculate the new account balance after the top-up
        double newBalance = Account.AccountBalance + value;

        // Check if the new balance is valid (not less than zero)
        if (newBalance < 0)
        {
            PrettyPrint.Info("Balance can't be less than zero");
            return;
        }

        // Update the account balance
        Account.UpdateAccountBalance(newBalance);
        PrettyPrint.Success("Successfully updated account balance");
    }

    private void TransactionCreate()
    {   
        // Prompt the user to input the name for the transaction
        string name = ReadInput(
            $"? How should this entry be called:", ""
        );
    
        // Prompt the user to input the amount for the transaction
        string amount = ReadInput(
            $"? How much did you spend:", ""
        );
    
        // Check if either name or amount is not provided
        if (string.IsNullOrWhiteSpace(amount) || string.IsNullOrWhiteSpace(name))
        {
            PrettyPrint.Info("Please provide both name and amount.");
            return;
        }
    
        // Check if the provided amount is a valid double
        if (!double.TryParse(amount, out double value))
        {
            PrettyPrint.Info("Amount should be a valid number");
            return;
        }
    
        // Add the new transaction to the database
        database.AddTransaction(new Transaction(value, name, Account.AccountName));
        PrettyPrint.Success("Successfully added transaction");
    }

    private void TransactionRemove()
    {
        // Check if there are no transactions for the current account
        if (database.Transactions.All(transaction => transaction.AccountName != Account.AccountName))
        {
            PrettyPrint.Info("There are no transactions to delete");
            return;
        }
    
        // Prompt the user to input the transaction ID
        string input = ReadInput($"? What transaction should be deleted (type transaction id):", "");

        // Check if the input is a valid integer (transaction ID)
        if (!int.TryParse(input, out int index))
        {
            PrettyPrint.Info("Not a valid integer");
            return;
        }
    
        // Check if the index is within the valid range
        if (index <= 0 || index > database.Transactions.Count)
        {
            PrettyPrint.Info("Transaction does not exist");
            return;
        }

        // Check if the transaction belongs to the current account
        if (database.Transactions[index - 1].AccountName != Account.AccountName)
        {
            PrettyPrint.Info("You can't manage transactions from other accounts");
            return;
        }

        // Remove the selected transaction from the database
        database.RemoveTransactionAt(index - 1);
        PrettyPrint.Success("Successfully deleted the transaction");
    }

    
    private void TransactionCommand(string[] args)
    {
        switch (args.Length)
        {
            // Check if there are too many arguments
            case >= 2:
                PrettyPrint.Info("Too many arguments");
                break;
            // Check if no arguments are provided, show all accounts then
            case 0:
                ShowTransactions();
                break;
            default:
                // Process the provided argument
                switch (args[0])
                {
                    case "remove":
                        TransactionRemove();
                        break;
                    case "create":
                        TransactionCreate();
                        break;
                    default:
                        // Display an error message for an invalid command
                        PrettyPrint.Info("Invalid command. Type 'help' for a list of commands.");
                        break;
                }
                break;
        }
    }

    private void BalanceCommand(string[] args)
    {
        // Display the current account balance
        PrettyPrint.Info($"Account balance: {Account.AccountBalance} {database.PreferredCurrency}");
    }

}
