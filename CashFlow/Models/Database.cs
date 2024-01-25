namespace CashFlow.CashFlow.Models;

using CashFlow.Utils;

class Database
{

    private Account[] accounts = {new Account("main", 500) };
    private Transaction[] transactions = {new Transaction(200, "main") };

    public Account GetAccount(string name)
    {
        foreach (Account account in accounts)
        {
            if (account.Name() == name)
                return account;
        }

        throw new AccountDoesNotExist("Account does not exist");
    }
    
    public bool AccountExist(string name) => accounts.Any(account => account.Name() == name);
    
    public Dictionary<string, object>? GetKey(string key) => database[key];
    public void SetKey(string key, Dictionary<string, object> value) => database[key] = value;
    public void RemoveKey(string key) => database.Remove(key);
    public DatabaseObject GetAccounts() => database["accounts"];
}