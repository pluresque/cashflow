namespace CashFlow.CashFlow.Models;

public class Account
{
    public string AccountName { get; private set; }
    public double AccountBalance { get; private set; }
    
    public Account(string accountName, double accountBalance)
    {
        // Set the account name and balance during object creation
        AccountName = accountName;
        AccountBalance = accountBalance;
    }


    public void UpdateAccountBalance(double newBalance)
    {
        // Check if the new balance is valid (not less than zero)
        if (newBalance < 0)
        {
            throw new ArgumentException("Account balance can't be less than zero");
        }

        // Update the account balance
        AccountBalance = newBalance;
    }
    
    public override string ToString()
    {
        // Function to convert object to string
        return $"{AccountName}:{AccountBalance}";
    }
    
    public static Account FromString(string input)
    {
        // Split the input string into parts using ':'
        string[] parts = input.Split(':');

        // Check if the input has the expected format
        if (parts.Length != 2)
        {
            throw new ArgumentException("Invalid input format for creating Account object");
        }

        // Extract account name and balance from the parts
        string accountName = parts[0];

        // Check if parsing the balance as double is successful
        if (!double.TryParse(parts[1], out var accountBalance))
        {
            throw new ArgumentException("Invalid input format for creating Account object");
        }

        // Create and return a new Account object
        return new Account(accountName, accountBalance);
    }
}
