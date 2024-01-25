namespace CashFlow.CashFlow.Models;

public class Transaction
{
    // Properties
    public double Amount { get; private set; }
    public string Name { get; private set; }
    public string AccountName { get; private set; }
    public DateTime TransactionTime { get; private set; }

    // Constructor
    public Transaction(double amount, string name, string accountName)
    {
        Amount = amount;
        Name = name;
        AccountName = accountName;
        TransactionTime = DateTime.Now; // Set current time by default
    }

    // Convert the transaction to a string in the specified format
    public override string ToString()
    {
        return $"{AccountName}:{Name}:{Amount}:{TransactionTime:yyyy-MM-dd HH/mm/ss}";
    }

    // Create a Transaction object from a string in the specified format
    public static Transaction FromString(string input)
    {
        string[] parts = input.Split(':');
        if (parts.Length != 4)
            throw new ArgumentException("Invalid input format for creating Transaction object");
        
        string accountName = parts[0];
        string purchase = parts[1];

        if (!double.TryParse(parts[2], out var amount))
            throw new ArgumentException("Invalid input format for creating Transaction object");
        
        DateTime transactionTime = DateTime.ParseExact(parts[3], "yyyy-MM-dd HH/mm/ss", null);
        return new Transaction(amount, purchase, accountName) { TransactionTime = transactionTime };
    }
}
