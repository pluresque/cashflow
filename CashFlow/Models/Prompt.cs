namespace CashFlow.CashFlow.Models;

using Utils;

abstract class Prompt
{
    protected Dictionary<string, Action<string[]>> commands = null!;
    protected readonly Database database = new();

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

        if (!commands.ContainsKey(command))
            AppLogger.Info("Invalid command. Type 'help' for a list of commands.");
        else
            commands[command](inputParts.Length > 1 ? inputParts[1..] : Array.Empty<string>());

        return true; // Continue
    }

    protected void HelpCommand(string[] args)
    {
        AppLogger.Info("Available Commands:");
        foreach (var cmd in commands.Keys)
        {
            Console.WriteLine($"- {cmd}");
        }
    }
}
