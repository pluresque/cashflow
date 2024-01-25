using System.Text.Json;

namespace CashFlow.CashFlow.Utils;

public class JsonParser
{
    private readonly string json;

    public JsonParser(string json)
    {
        this.json = json;
    }

    public JsonParser(FileInfo file, bool createIfNotExist = false)
    {
        if (file == null)
        {
            throw new ArgumentNullException(nameof(file));
        }

        if (!file.Exists)
        {
            if (createIfNotExist)
            {
                using (StreamWriter writer = file.CreateText())
                {
                    writer.Write("{}"); // Create an empty JSON object if the file does not exist
                }
            }
            else
            {
                throw new FileNotFoundException(
                    "The specified JSON file does not exist.",
                    file.FullName
                );
            }
        }

        using StreamReader reader = file.OpenText();
        this.json = reader.ReadToEnd();
    }

    public object Parse()
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("JSON string is empty or null.");
        }

        int index = 0;
        return ParseValue(ref index);
    }

    private object ParseValue(ref int index)
    {
        SkipWhiteSpace(ref index);

        char currentChar = json[index];
        return currentChar switch
        {
            '{' => ParseObject(ref index),
            '[' => ParseArray(ref index),
            '"' => ParseString(ref index),
            't' => ParseTrue(ref index),
            'f' => ParseFalse(ref index),
            'n' => ParseNull(ref index),
            _ => ParseNumber(ref index)
        };
    }

    private void SkipWhiteSpace(ref int index)
    {
        while (index < json.Length && char.IsWhiteSpace(json[index]))
        {
            index++;
        }
    }

    private Dictionary<string, object> ParseObject(ref int index)
    {
        index++; // Skip '{'
        Dictionary<string, object> result = new Dictionary<string, object>();

        while (json[index] != '}')
        {
            SkipWhiteSpace(ref index);

            // Parse key
            string key = ParseString(ref index);

            SkipWhiteSpace(ref index);
            if (json[index] != ':')
            {
                throw new InvalidOperationException(
                    $"Expected ':' at position {index}, but found '{json[index]}'."
                );
            }

            index++; // Skip ':'

            // Parse value
            object value = ParseValue(ref index);

            result.Add(key, value);

            SkipWhiteSpace(ref index);

            if (json[index] == ',')
            {
                index++; // Skip ','
            }
            else if (json[index] != '}')
            {
                throw new InvalidOperationException(
                    $"Expected ',' or '}}' at position {index}, but found '{json[index]}'."
                );
            }
        }

        index++; // Skip '}'
        return result;
    }

    private List<object> ParseArray(ref int index)
    {
        index++; // Skip '['
        List<object> result = new List<object>();

        while (json[index] != ']')
        {
            SkipWhiteSpace(ref index);

            // Parse value
            object value = ParseValue(ref index);

            result.Add(value);

            SkipWhiteSpace(ref index);

            if (json[index] == ',')
            {
                index++; // Skip ','
            }
            else if (json[index] != ']')
            {
                throw new InvalidOperationException(
                    $"Expected ',' or ']' at position {index}, but found '{json[index]}'."
                );
            }
        }

        index++; // Skip ']'
        return result;
    }

    private string ParseString(ref int index)
    {
        index++; // Skip '"'
        int startIndex = index;

        while (index < json.Length && json[index] != '"')
        {
            index++;
        }

        if (index >= json.Length)
        {
            throw new InvalidOperationException("Unterminated string.");
        }

        string result = json.Substring(startIndex, index - startIndex);
        index++; // Skip '"'
        return result;
    }

    private bool ParseTrue(ref int index)
    {
        string trueString = json.Substring(index, 4);
        if (trueString == "true")
        {
            index += 4;
            return true;
        }

        throw new InvalidOperationException($"Expected 'true' at position {index}.");
    }

    private bool ParseFalse(ref int index)
    {
        string falseString = json.Substring(index, 5);
        if (falseString == "false")
        {
            index += 5;
            return false;
        }

        throw new InvalidOperationException($"Expected 'false' at position {index}.");
    }

    private object ParseNull(ref int index)
    {
        string nullString = json.Substring(index, 4);
        if (nullString == "null")
        {
            index += 4;
            return null;
        }
        else
        {
            throw new InvalidOperationException($"Expected 'null' at position {index}.");
        }
    }

    private object ParseNumber(ref int index)
    {
        int startIndex = index;

        while (
            index < json.Length
            && (
                char.IsDigit(json[index])
                || json[index] == '-'
                || json[index] == '.'
                || json[index] == 'e'
                || json[index] == 'E'
            )
        )
        {
            index++;
        }

        string numberString = json.Substring(startIndex, index - startIndex);
        if (int.TryParse(numberString, out int intValue))
        {
            return intValue;
        }

        if (double.TryParse(numberString, out double doubleValue))
        {
            return doubleValue;
        }

        throw new InvalidOperationException(
            $"Invalid number format at position {startIndex}: {numberString}"
        );
    }
}
