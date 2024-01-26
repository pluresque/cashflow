namespace CashFlow.CashFlow.Utils;

public static class PrettyPrint
{
    public static void Info(string message)
    {
        Log("!", message, ConsoleColor.Blue);
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
}
