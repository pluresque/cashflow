using System.Diagnostics;

namespace CashFlow.CashFlow.Models;

using Utils;
using Newtonsoft.Json;

class Database
{
    public List<Account> Accounts { get; private set; }
    public List<Transaction> Transactions { get; private set; }

    public string FilePath { get; private set; }
    public Dictionary<string, object> Settings { get; private set; }

    public Database(string filePath)
    {
        // Create empty lists in order to populate them later on
        Accounts = new();
        Transactions = new();
        
        // File path to database
        FilePath = filePath;
        
        // Update database
        UpdateDatabase();
    }

    private void UpdateDatabase()
    {
        // Read JSON from file
        string jsonText = File.ReadAllText(FilePath);

        // Deserialize JSON to object
        DatabaseSchema? databaseObject = JsonConvert.DeserializeObject<DatabaseSchema>(jsonText);

        // Populate accounts from the deserialized data
        if (databaseObject == null) 
            throw new DatabaseInitializationException("Not all needed keys are in database");
        
        foreach (string accountString in databaseObject.accounts)
        {
            Accounts.Add(Account.FromString(accountString));
        }

        // Populate transactions from the deserialized data
        foreach (string transactionString in databaseObject.transactions)
        {
            Transactions.Add(Transaction.FromString(transactionString));
        }

        // Populate settings from the deserialized data
        Settings = databaseObject.settings;
    }

    private void SaveDatabase()
    {
        // Convert accounts to string representation
        List<string> accountsList = Accounts.Select(account => account.ToString()).ToList();

        // Convert transactions to string representation
        List<string> transactionList = Transactions.Select(transaction => transaction.ToString()).ToList();

        // Create a DatabaseSchema object with the collected data
        DatabaseSchema databaseObject = new DatabaseSchema()
        {
            accounts = accountsList,
            transactions = transactionList,
            settings = Settings
        };

        try
        {
            // Convert the object to a JSON string with indentation for readability
            string jsonString = JsonConvert.SerializeObject(databaseObject, Formatting.Indented);

            // Write the JSON string to the database file
            File.WriteAllText(FilePath, jsonString);
        }
        catch (Exception ex)
        {
            // Handle exceptions related to file writing or serialization
            throw new DatabaseSaveException("Error during database save");
        }
    }
    
    public Account GetAccount(string name)
    {
        foreach (Account account in Accounts)
        {
            if (account.Name == name)
                return account;
        }

        throw new AccountDoesNotExist("Account does not exist");
    }

    public bool UpdateAccount(Account account)
    {
        for (int i = 0; i < Accounts.Count; i++)
        {
            if (Accounts[i].Name != account.Name)
                continue;
            Accounts[i] = account;
            SaveDatabase();
            return true;
        }

        return false;
    }

    public bool RemoveAccount(string name)
    {
        for (int i = 0; i < Accounts.Count; i++)
        {
            if (Accounts[i].Name != name)
                continue;
            RemoveAccountAt(i);
            SaveDatabase();
            return true;
        }
        return false;
    }

    public bool AddAccount(Account account)
    {
        if (AccountExist(account.Name))
            return false;

        Accounts.Add(account);
        SaveDatabase();
        return true;
    }

    public void SetTransactionsPerPage(int value)
    {
        if (value is < 30 or > 100)
            throw new TransactionPerPageTooBig("Too high/low value for transactionPerPage setting");

        Settings["transactionsPerPage"] = value;
        SaveDatabase();
    }

    public void SetPreferredCurrency(string currency)
    {
        if (!CheckCurrency.IsValidCurrency(currency))
            throw new UnknownCurrency("This currency is not supported");

        Settings["preferredCurrency"] = currency.ToUpper();
        SaveDatabase();
    }

    public void AddTransaction(Transaction transaction)
    {
        Transactions.Add(transaction);
        SaveDatabase();
    }

    public void RemoveTransactionAt(int index)
    {
        Transactions.RemoveAt(index);
        SaveDatabase();
    }

    public void RemoveAccountAt(int index)
    {
        Accounts.RemoveAt(index);
        SaveDatabase();
    }

    public bool AccountExist(string name) => Accounts.Any(acc => acc.Name == name);
    public bool AccountsEmpty() => Accounts.Count == 0;
    
    public object preferredCurrency => Settings["preferredCurrency"];
    public object transactionsPerPage => Settings["transactionsPerPage"];
}

public class DatabaseSchema
{
    public required List<string> accounts { get; set; }
    public required List<string> transactions { get; set; }
    public required Dictionary<string, object> settings { get; set; }
}
