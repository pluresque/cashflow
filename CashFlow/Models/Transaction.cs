namespace CashFlow.CashFlow.Models;

public class Transaction: DatabaseObject
{
    // Properties
    private double Amount;
    private string AccountName;
    private DateTime TransactionTime;

    // Constructor
    public Transaction(double amount, string accountName)
    {
        Amount = amount;
        AccountName = accountName;
        TransactionTime = DateTime.Now; // Set current time by default
    }

    // Convert the transaction to a string in the specified format
    public override string ToString()
    {
        return $"{AccountName}:{Amount}:{TransactionTime:yyyy-MM-dd HH:mm:ss}";
    }

    // Create a Transaction object from a string in the specified format
    public static Transaction FromString(string input)
    {
        string[] parts = input.Split(':');
        if (parts.Length != 3) 
            throw new ArgumentException("Invalid input format for creating Account object");
        
        string accountName = parts[0];

        if (!double.TryParse(parts[1], out var amount))
            throw new ArgumentException("Invalid input format for creating Account object");
        
        DateTime transactionTime = DateTime.ParseExact(parts[2], "yyyy-MM-dd HH:mm:ss", null);
        return new Transaction(amount, accountName) { TransactionTime = transactionTime };
    }

    public string Name() => AccountName;
}