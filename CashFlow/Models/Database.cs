namespace CashFlow.CashFlow.Models;

using Utils;
using Newtonsoft.Json;

class Database
{
    public List<Account> accounts = new List<Account>();
    public List<Transaction> transactions = new List<Transaction>();
    public Dictionary<string, object> settings = new Dictionary<string, object>();

    public Database()
    {
        UpdateDatabase();
    }

    public void UpdateDatabase()
    {
        accounts = new List<Account>();
        transactions = new List<Transaction>();
        settings = new Dictionary<string, object>();
        
        // Read JSON from file
        string jsonText = File.ReadAllText("database.json");

        // Deserialize JSON to object
        DatabaseSchema? databaseObject = JsonConvert.DeserializeObject<DatabaseSchema>(jsonText);
        if (databaseObject is null)
        {
            throw new BadDatabaseFile("File does not contain all needed keys");
        }

        foreach (string account in databaseObject.accounts)
        {
            accounts.Add(Account.FromString(account));
        }
        
        foreach (string transaction in databaseObject.transactions)
        {
            transactions.Add(Transaction.FromString(transaction));
        }

        settings = databaseObject.settings;
    }

    public void SaveDatabase()
    {
        List<string> accountsList = new List<string>();
        foreach (Account account in accounts)
        {
            accountsList.Add(account.ToString());
        }
        
        List<string> transactionList = new List<string>();
        foreach (Transaction transaction in transactions)
        {
            transactionList.Add(transaction.ToString());
        }
        
        DatabaseSchema databaseObject = new DatabaseSchema()
        {
            accounts = accountsList,
            transactions = transactionList,
            settings = settings
        };
        
        // Convert the object to a JSON string
        string jsonString = JsonConvert.SerializeObject(databaseObject, Formatting.Indented);
        
        // Write the JSON string to a file
        File.WriteAllText("database.json", jsonString);
    }
    
    public Account GetAccount(string name)
    {
        foreach (Account account in accounts)
        {
            if (account.Name == name)
                return account;
        }

        throw new AccountDoesNotExist("Account does not exist");
    }

    public bool UpdateAccount(Account account)
    {
        for (int i = 0; i < accounts.Count; i++)
        {
            if (accounts[i].Name != account.Name)
                continue;
            accounts[i] = account;
            SaveDatabase();
            return true;
        }

        return false;
    }

    public bool RemoveAccount(string name)
    {
        for (int i = 0; i < accounts.Count; i++)
        {
            if (accounts[i].Name != name)
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

        accounts.Add(account);
        SaveDatabase();
        return true;
    }

    public void SetTransactionsPerPage(int value)
    {
        if (value is < 30 or > 100)
            throw new TransactionPerPageTooBig("Too high/low value for transactionPerPage setting");

        settings["transactionsPerPage"] = value;
        SaveDatabase();
    }

    public void SetPreferredCurrency(string currency)
    {
        if (currency is not ("PLN" or "USD" or "EUR"))
            throw new UnknownCurrency("This currency is not supported");

        settings["preferredCurrency"] = currency;
        SaveDatabase();
    }

    public void AddTransaction(Transaction transaction)
    {
        transactions.Add(transaction);
        SaveDatabase();
    }

    public void RemoveTransactionAt(int index)
    {
        transactions.RemoveAt(index);
        SaveDatabase();
    }

    public void RemoveAccountAt(int index)
    {
        Accounts.RemoveAt(index);
        SaveDatabase();
    }

    public bool AccountExist(string name) => accounts.Any(acc => acc.Name == name);

    public List<Account> Accounts => accounts;
    public List<Transaction> Transactions => transactions;
    public object preferredCurrency => settings["preferredCurrency"];
    public object transactionsPerPage => settings["transactionsPerPage"];
}

public class DatabaseSchema
{
    public required List<string> accounts { get; set; }
    public required List<string> transactions { get; set; }
    public required Dictionary<string, object> settings { get; set; }
}
