using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

public class Settings
{
    private static readonly JsonSerializerSettings JsonSettings;

    static Settings()
    {
        JsonSettings = new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented
        };
    }

    [JsonIgnore]
    public const string SettingFile = "!Settings.txt";

    public static string AccountServerAddressIP = "127.0.0.1";
    public static ushort AccountServerAddressPort = 7_000;

    public static string SaveAccountName = string.Empty;
    public static string SaveArea = string.Empty;

    public static void Load()
    {
        if (!File.Exists(SettingFile))
            return;

        var json = File.ReadAllText(SettingFile);

        var source = JsonConvert.DeserializeObject<JToken>(json, JsonSettings);
        var destinationProperties = typeof(Settings).GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (JProperty prop in source)
        {
            var destinationProp = destinationProperties
                .SingleOrDefault(p => p.Name.Equals(prop.Name, StringComparison.OrdinalIgnoreCase));
            if (destinationProp == null) continue;
            if (destinationProp.IsLiteral && !destinationProp.IsInitOnly) continue; // Is a const field
            var value = ((JValue)prop.Value).Value;
            //The ChangeType is required because JSON.Net will deserialise
            //numbers as long by default
            destinationProp.SetValue(null, Convert.ChangeType(value, destinationProp.FieldType));
        }
    }

    public static void Save()
    {
        var myType = typeof(Settings);
        var TypeBlob = myType.GetFields()
            .Where(x => !(x.IsLiteral && !x.IsInitOnly))
            .ToDictionary(x => x.Name, x => x.GetValue(null));
        var json = JsonConvert.SerializeObject(TypeBlob, JsonSettings);
        File.WriteAllText(SettingFile, json);
    }
}
