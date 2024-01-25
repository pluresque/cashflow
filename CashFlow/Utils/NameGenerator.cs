namespace CashFlow.CashFlow.Utils;

public static class NameGenerator
{
    private static Random random = new();

    private static string[] firstWords =
    {
        "Silly",
        "Happy",
        "Angry",
        "Crazy",
        "Funny",
        "Giggly",
        "Cheerful",
        "Playful"
    };
    private static string[] secondWords =
    {
        "Cat",
        "Dog",
        "Dinosaur",
        "Elephant",
        "Monkey",
        "Giraffe",
        "Penguin",
        "Cockroach"
    };

    public static string GenerateFunnyName()
    {
        string firstName = GetRandomElement(firstWords);
        string secondName = GetRandomElement(secondWords);

        return $"{firstName} {secondName}";
    }

    private static string GetRandomElement(string[] array)
    {
        int index = random.Next(array.Length);
        return array[index];
    }
}
