using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SqlD.Serialiser;

public class JsonSerialiser
{
    static JsonSerialiser()
    {
        Settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };
    }

    public static JsonSerializerSettings Settings { get; set; }

    public static string Serialise<T>(T input)
    {
        return JsonConvert.SerializeObject(input, settings: Settings);
    }

    public static T Deserialise<T>(string input)
    {
        return JsonConvert.DeserializeObject<T>(input);
    }
}