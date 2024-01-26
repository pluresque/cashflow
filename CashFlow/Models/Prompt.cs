namespace CashFlow.CashFlow.Models;

using Utils;

abstract class Prompt
{
    protected Dictionary<string, Action<string[]>> Commands = null!;
    protected readonly Database database = new("database.json");

    public abstract void Run();

    protected static string ReadInput(string prompt, string defaultValue)
    {
        // Display the prompt and read the user's input
        Console.Write(prompt + " ");
        var answer = Console.ReadLine();
        
        // Use the defaultValue if the user input is null, empty, or consists of whitespace
        return string.IsNullOrWhiteSpace(answer) ? defaultValue : answer.Trim();
    }
    
    protected bool ParseInput(string input)
    {
        // Split the input into parts
        string[] inputParts = input.Split(' ');

        // Extract the command (first part) and convert it to lowercase
        string command = inputParts[0].ToLower();

        // Check if the command is "exit"
        if (command == "exit")
        {
            // Return false to indicate that the program should exit
            return false;
        }

        // Check if the command is valid
        if (!Commands.ContainsKey(command))
        {
            // Inform the user about an invalid command
            PrettyPrint.Info("Invalid command. Type 'help' for a list of commands.");
        }
        else
        {
            // Execute the command with its arguments
            Commands[command](inputParts.Length > 1 ? inputParts[1..] : Array.Empty<string>());
        }

        // Return true to indicate that the program should continue
        return true;
    }

    protected void HelpCommand(string[] args)
    {
        // Display a header for available commands
        PrettyPrint.Info("Available Commands:");

        // Iterate through each command and print its name
        foreach (var cmd in Commands.Keys)
        {
            Console.WriteLine($"- {cmd}");
        }
    }

}
