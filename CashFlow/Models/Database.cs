namespace CashFlow.CashFlow.Models;

using CashFlow.Utils;

class Database
{
    private List<Account> accounts = new() { new Account("main", 500) };
    private List<Transaction> transactions = new() { new Transaction(200, "main") };
    private Dictionary<string, object> settings =
        new() { { "transactionsPerPage", 30 }, { "preferredCurrency", "PLN" } };

    public Database()
    {
        JsonParser databaseJson = new JsonParser("database.json");
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
            return true;
        }
        return false;
    }

    public bool AddAccount(Account account)
    {
        if (AccountExist(account.Name))
            return false;

        accounts.Add(account);
        return true;
    }

    public void SetTransactionsPerPage(int value)
    {
        if (value is < 30 or > 100)
            throw new TransactionPerPageTooBig("Too high/low value for transactionPerPage setting");

        settings["transactionsPerPage"] = value;
    }

    public void SetPreferredCurrency(string currency)
    {
        if (currency is not ("PLN" or "USD" or "EUR"))
            throw new UnknownCurrency("This currency is not supported");

        settings["preferredCurrency"] = currency;
    }

    public void AddTransaction(Transaction transaction) => transactions.Add(transaction);

    public void RemoveTransactionAt(int index) => transactions.RemoveAt(index);

    public void RemoveAccountAt(int index) => Accounts.RemoveAt(index);

    public bool AccountExist(string name) => accounts.Any(acc => acc.Name == name);

    public List<Account> Accounts => accounts;
    public List<Transaction> Transactions => transactions;
    public object preferredCurrency => settings["preferredCurrency"];
    public object transactionsPerPage => settings["transactionsPerPage"];
}
