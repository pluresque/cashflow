namespace CashFlow.CashFlow.Models;

using Utils;

abstract class Prompt
{
    protected Dictionary<string, Action<string[]>> Commands = null!;
    protected readonly Database database = new("database.json");

    public abstract void Run();

    protected static string ReadInput(string prompt, string defaultValue)
    {
        Console.Write(prompt + " ");
        var answer = Console.ReadLine();
        return string.IsNullOrWhiteSpace(answer) ? defaultValue : answer.Trim();
    }

    protected bool ParseInput(string input)
    {
        string[] inputParts = input.Split(' ');
        string command = inputParts[0].ToLower();

        if (command == "exit")
            return false; // Meaning exit

        if (!Commands.ContainsKey(command))
            AppLogger.Info("Invalid command. Type 'help' for a list of commands.");
        else
            Commands[command](inputParts.Length > 1 ? inputParts[1..] : Array.Empty<string>());

        return true; // Continue
    }

    protected void HelpCommand(string[] args)
    {
        AppLogger.Info("Available Commands:");
        foreach (var cmd in Commands.Keys)
        {
            Console.WriteLine($"- {cmd}");
        }
    }
}
