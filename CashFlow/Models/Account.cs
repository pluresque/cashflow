namespace CashFlow.CashFlow.Models;

public class Account
{
    // Properties
    public string AccountName { get; private set; }
    public double AccountBalance { get; private set; }

    // Constructor
    public Account(string accountName, double accountBalance)
    {
        AccountName = accountName;
        AccountBalance = accountBalance;
    }

    // Function to convert object to string
    public override string ToString()
    {
        return $"{AccountName}:{AccountBalance}";
    }

    // Function to create an object from a string
    public static Account FromString(string input)
    {
        string[] parts = input.Split(':');
        if (parts.Length != 2)
            throw new ArgumentException("Invalid input format for creating Account object");
        string accountName = parts[0];

        if (!double.TryParse(parts[1], out var accountBalance))
            throw new ArgumentException("Invalid input format for creating Account object");

        return new Account(accountName, accountBalance);

        // If the input format is incorrect, return null or throw an exception based on your preference.
    }

    public string Name => AccountName;
}
