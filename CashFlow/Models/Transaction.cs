using System.Globalization;

namespace CashFlow.CashFlow.Models;

public class Transaction
{
    // Properties
    public double Amount { get; private set; }
    public string Name { get; private set; }
    public string AccountName { get; private set; }
    public DateTime TransactionTime { get; private set; }
    
    public Transaction(double amount, string name, string accountName)
    {
        Amount = amount;
        Name = name;
        AccountName = accountName;
        TransactionTime = DateTime.Now; // Set current time by default
    }
    
    public override string ToString()
    {
        // Convert the transaction to a string in the specified format
        return $"{AccountName}:{Name}:{Amount}:{TransactionTime:yyyy-MM-dd HH/mm/ss}";
    }
    
    public static Transaction FromString(string input)
    {
        // Split the input into parts
        string[] parts = input.Split(':');

        // Check if the input has the correct number of parts
        if (parts.Length != 4)
        {
            throw new ArgumentException("Invalid input format for creating Transaction object");
        }

        // Extract information from the parts
        string accountName = parts[0];
        string purchase = parts[1];

        // Parse the amount from the input
        if (!double.TryParse(parts[2], out var amount))
        {
            throw new ArgumentException("Invalid input format for creating Transaction object");
        }
        
        // Parse the transaction time from the input
        if (!DateTime.TryParseExact(parts[3], "yyyy-MM-dd HH/mm/ss", null, DateTimeStyles.None, out var transactionTime))
        {
            throw new ArgumentException("Invalid input format for creating Transaction object");
        }
        
        // Create and return the Transaction object
        return new Transaction(amount, purchase, accountName) { TransactionTime = transactionTime };
    }

}
