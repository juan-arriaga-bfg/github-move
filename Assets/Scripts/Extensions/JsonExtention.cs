using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class JsonExtensions
{
    public static void PopulateObject<T>(this JToken value, T target) where T : class
    {
        using (var sr = value.CreateReader())
        {
            JsonSerializer.CreateDefault().Populate(sr, target); // Uses the system default JsonSerializerSettings
        }
    }
}