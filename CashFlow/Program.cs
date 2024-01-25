using CashFlow.CashFlow.Utils;
using CashFlow.CashFlow.Controllers;

class Program
{
    static void Main()
    {
        var settingsSchema = new Dictionary<string, Type>
        {
            { "preferred_currency", typeof(string) }
        };
        
        //JsonManager database = new JsonManager("database.json", schema);
        //JsonManager settings = new JsonManager("settings.json", schema);

        App app = new App();
        app.Run();
    }
}
 