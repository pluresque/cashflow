namespace CashFlow.CashFlow.Utils;

public static class AppLogger
{
    public static void Debug(string message)
    {
        if (!IsEnvironmentVariableSet())
            return;
        Log("Debug", message, ConsoleColor.Blue);
    }

    public static void Error(string message)
    {
        if (!IsEnvironmentVariableSet())
            return;
        Log("Error", message, ConsoleColor.Red);
    }

    public static void Info(string message)
    {
        Log("!", message, ConsoleColor.Blue);
    }

    public static void Question(string message)
    {
        Log("", message, ConsoleColor.Yellow, false);
    }

    public static void Success(string message)
    {
        Log("\u2714", message, ConsoleColor.Green);
    }

    public static void Log(string prompt, string message, ConsoleColor color, bool newline = true)
    {
        Console.ForegroundColor = color;
        Console.Write(prompt + " ");
        Console.ResetColor(); // Reset color to default
        Console.Write(message);

        if (newline)
            Console.Write("\n");
    }

    private static bool IsEnvironmentVariableSet()
    {
        string value = Environment.GetEnvironmentVariable("CASHFLOW_DEBUG")!;
        return !string.IsNullOrEmpty(value) && value.ToLower() == "true";
    }
}
