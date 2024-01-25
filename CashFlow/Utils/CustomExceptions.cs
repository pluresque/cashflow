namespace CashFlow.CashFlow.Utils;

// Custom exception for API communication errors
public class ApiException : Exception
{
    public ApiException(string message)
        : base(message) { }
}

public class AccountDoesNotExist : Exception
{
    public AccountDoesNotExist(string message)
        : base(message) { }
}

public class TransactionPerPageTooBig : Exception
{
    public TransactionPerPageTooBig(string message)
        : base(message) { }
}

public class UnknownCurrency : Exception
{
    public UnknownCurrency(string message)
        : base(message) { }
}