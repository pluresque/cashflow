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
        Accounts = new List<Account>();
        Transactions = new List<Transaction>();
        Settings = new Dictionary<string, object>();
        
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
        catch (Exception)
        {
            // Handle exceptions related to file writing or serialization
            throw new DatabaseSaveException("Error during database save");
        }
    }
    
    public Account GetAccount(string name)
    {
        // Iterate through all accounts
        foreach (Account account in Accounts)
        {
            // Check if the current account matches the provided name
            if (account.AccountName == name)
            {
                // Return the matching account
                return account;
            }
        }

        // Throw an exception if no matching account is found
        throw new AccountDoesNotExist("Account does not exist");
    }

    public bool UpdateAccount(Account updatedAccount)
    {
        // Iterate through all accounts
        for (int i = 0; i < Accounts.Count; i++)
        {
            // Check if the current account matches the updated account's name
            if (Accounts[i].AccountName == updatedAccount.AccountName)
            {
                // Update the account in the list
                Accounts[i] = updatedAccount;

                // Save the database after the update
                SaveDatabase();

                // Return true to indicate a successful update
                return true;
            }
        }

        // Return false if no matching account is found
        return false;
    }

    public bool RemoveAccount(string name)
    {
        // Iterate through all accounts
        for (int i = 0; i < Accounts.Count; i++)
        {
            // Check if the current account matches the provided name
            if (Accounts[i].AccountName == name)
            {
                // Remove the account at the current index
                RemoveAccountAt(i);

                // Save the database after the removal
                SaveDatabase();

                // Return true to indicate a successful removal
                return true;
            }
        }

        // Return false if no matching account is found
        return false;
    }

    public bool AddAccount(Account newAccount)
    {
        // Check if an account with the same name already exists
        if (AccountExist(newAccount.AccountName))
        {
            // Return false to indicate that the account was not added
            return false;
        }

        // Add the new account to the list
        Accounts.Add(newAccount);

        // Save the database after the addition
        SaveDatabase();

        // Return true to indicate a successful addition
        return true;
    }

    public void SetTransactionsPerPage(int newPerPageValue)
    {
        // Check if the new value is within the valid range
        if (newPerPageValue < 10 || newPerPageValue > 100)
        {
            // Throw an exception if the value is out of range
            throw new TransactionPerPageTooBig("Invalid value for transactionsPerPage setting");
        }

        // Update the transactions per page setting
        Settings["transactionsPerPage"] = newPerPageValue;

        // Save the database after the update
        SaveDatabase();
    }
    
    public void SetPreferredCurrency(string newCurrency)
    {
        // Check if the new currency is valid
        if (!CheckCurrency.IsValidCurrency(newCurrency))
        {
            // Throw an exception if the currency is not supported
            throw new UnknownCurrency("This currency is not supported");
        }

        // Update the preferred currency setting (convert to uppercase for consistency)
        Settings["preferredCurrency"] = newCurrency.ToUpper();

        // Save the database after the update
        SaveDatabase();
    }

    public void AddTransaction(Transaction newTransaction)
    {
        // Add the new transaction to the list
        Transactions.Add(newTransaction);

        // Save the database after the addition
        SaveDatabase();
    }

    public void RemoveTransactionAt(int indexToRemove)
    {
        // Remove the transaction at the specified index
        Transactions.RemoveAt(indexToRemove);

        // Save the database after the removal
        SaveDatabase();
    }
    
    public void RemoveAccountAt(int indexToRemove)
    {
        // Remove the account at the specified index
        Accounts.RemoveAt(indexToRemove);

        // Save the database after the removal
        SaveDatabase();
    }

    public bool AccountExist(string name) => Accounts.Any(acc => acc.AccountName == name);
    
    public bool AccountsEmpty() => Accounts.Count == 0;
    
    public bool TransactionsEmpty() => Transactions.Count == 0;
    
    public object PreferredCurrency => Settings["preferredCurrency"];
    
    public int TransactionsPerPage => (int)(long)Settings["transactionsPerPage"];
}

public class DatabaseSchema
{
    // null! is a workaround to be supported by NET 6.0 since required
    // is not available just yet
    public List<string> accounts { get; init; } = null!;
    public List<string> transactions { get; init; } = null!;
    public Dictionary<string, object> settings { get; init; } = null!;
}
