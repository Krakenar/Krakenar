namespace Krakenar.Core;

public static class ObjectExtensions
{
  public static readonly JsonSerializerOptions SerializerOptions = new();
  static ObjectExtensions()
  {
    SerializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  public static T DeepClone<T>(this T value) where T : notnull
  {
    Type type = value.GetType();
    string json = JsonSerializer.Serialize(value, type, SerializerOptions);
    return (T?)JsonSerializer.Deserialize(json, type, SerializerOptions) ?? throw new InvalidOperationException($"The value could not be deserialized: '{json}'.");
  }
}
