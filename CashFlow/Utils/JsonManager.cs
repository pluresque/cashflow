using System.Text.Json;

namespace CashFlow.CashFlow.Utils;


public class JsonManager
{
    private Dictionary<string, object> jsonObject;
    private Dictionary<string, Type>? schema;
    
    public Dictionary<string, object> JsonObject => jsonObject;
    public readonly string FilePath;

    public JsonManager(string filePath, Dictionary<string, Type>? schema = null)
    {
        this.schema = schema;
        FilePath = filePath;
        jsonObject = new Dictionary<string, object>();
        
        // Create the JSON file if it doesn't exist
        CreateJsonFile();

        // Read the existing JSON file content
        jsonObject = ReadJsonFile(File.ReadAllText(filePath));
        
        ValidateObject();
    }
    
    public object GetValue(string key)
    {
        // Try to get the value associated with the specified key
        if (jsonObject.TryGetValue(key, out var value))
        {
            return value;
        }

        Logger.Info($"Key '{key}' not found.");
        return null!;
    }
    
    public void SetValue(string key, object value)
    {
        // Update existing key
        // Add new key
        jsonObject[key] = value;
    }

    public void SaveJsonToFile()
    {
        try
        {
            // Convert the dictionary to a JSON string
            string jsonContent =
                JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = true, });

            // Write the JSON content to the specified file
            File.WriteAllText(FilePath, jsonContent);

            Logger.Info("JSON data saved successfully.");
        }
        catch (Exception ex)
        {
            Logger.Info($"Error saving JSON file: {ex.Message}");
        }
    }

    private void CreateJsonFile()
    {
        try
        {
            if (File.Exists(FilePath)) return;
            // Create the file with an empty JSON object
            File.WriteAllText(FilePath, "{}");
            Logger.Info($"JSON file {FilePath} created successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating JSON file: {ex.Message}");
        }
    }

    public static Dictionary<string, object> ReadJsonFile(string jsonContent)
    {
        try
        {
            // Parse the JSON string to a JsonDocument
            using JsonDocument document = JsonDocument.Parse(jsonContent);
            // Convert the JsonDocument to a dictionary
            return ParseJsonDocument(document.RootElement);
        }
        catch (Exception ex)
        {
            if (!ex.Message.Contains("isFinalBlock"))
            {
                // There seems to be a bug that spams with errors. They are not affecting code in any way,
                // they are just annoying.
                Logger.Info($"failed reading JSON file: {ex.Message}");
            }
        }
        return null!;
    }

    private static Dictionary<string, object> ParseJsonDocument(JsonElement jsonElement)
    {
        Dictionary<string, object> result = new Dictionary<string, object>();

        foreach (JsonProperty property in jsonElement.EnumerateObject())
        {
            switch (property.Value.ValueKind)
            {
                case JsonValueKind.Object:
                    result[property.Name] = ParseJsonDocument(property.Value);
                    break;
                case JsonValueKind.Array:
                    // You may need to implement specific handling for arrays if necessary
                    break;
                case JsonValueKind.String:
                    result[property.Name] = property.Value.GetString()!;
                    break;
                case JsonValueKind.Number:
                    result[property.Name] = property.Value.GetDecimal();
                    break;
                case JsonValueKind.True:
                    result[property.Name] = true;
                    break;
                case JsonValueKind.False:
                    result[property.Name] = false;
                    break;
                case JsonValueKind.Null:
                    result[property.Name] = null!;
                    break;
            }
        }

        return result;
    }
    
    private bool ValidateObject()
    {
        if (schema is null)
            return true;
        
        foreach (var (propertyName, expectedType) in schema)
        {
            if (!jsonObject.TryGetValue(propertyName, out var propertyValue))
            {
                return false;
            }

            if (!ValidateType(propertyValue, expectedType))
            {
                return false;
            }
            
            Console.WriteLine(expectedType);
        }

        return true;
    }

    private bool ValidateType(object value, Type expectedType)
    {
        return Type.GetTypeCode(expectedType) switch
        {
            TypeCode.String => value is string,
            TypeCode.Int32 => value is int,
            TypeCode.Boolean => value is bool,
            TypeCode.Object => true,
            _ => false
        };
    }
}