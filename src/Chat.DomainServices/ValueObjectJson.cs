using System.Text;
using System.Text.Json;
using DigiTFactory.Libraries.SeedWorks.TacticalPatterns;

namespace Chat.DomainServices;

/// <summary>
/// Сериализация изменённых Value Object-ов в JSON для журнала событий.
/// Рефлексия нужна потому, что значения объявлены интерфейсами (<see cref="IValueObject"/>).
/// </summary>
internal static class ValueObjectJson
{
    public static string Serialize(IDictionary<string, IValueObject> valueObjects)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();
            foreach (var (key, value) in valueObjects)
            {
                writer.WritePropertyName(key);
                WriteValue(writer, value, 0);
            }

            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static void WriteValue(Utf8JsonWriter writer, object? value, int depth)
    {
        switch (value)
        {
            case null:
                writer.WriteNullValue();
                return;
            case string text:
                writer.WriteStringValue(text);
                return;
            case bool flag:
                writer.WriteBooleanValue(flag);
                return;
            case int number:
                writer.WriteNumberValue(number);
                return;
            case long number:
                writer.WriteNumberValue(number);
                return;
            case Guid id:
                writer.WriteStringValue(id);
                return;
            case System.Collections.IEnumerable sequence:
                writer.WriteStartArray();
                foreach (var item in sequence)
                {
                    WriteValue(writer, item, depth + 1);
                }

                writer.WriteEndArray();
                return;
            default:
                if (depth > 8)
                {
                    writer.WriteNullValue();
                    return;
                }

                writer.WriteStartObject();
                foreach (var property in value.GetType().GetProperties())
                {
                    writer.WritePropertyName(property.Name);
                    WriteValue(writer, property.GetValue(value), depth + 1);
                }

                writer.WriteEndObject();
                return;
        }
    }
}
