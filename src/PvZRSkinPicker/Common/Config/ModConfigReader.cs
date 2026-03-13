namespace PvZRSkinPicker.Config;

using System.Diagnostics.Contracts;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

internal static class ModConfigReader
{
    private const string FormatVersionKey = "format_version";

    private static readonly JsonLoadSettings LoadSettings = new()
    {
        CommentHandling = CommentHandling.Ignore,
        DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error,
    };

    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        MissingMemberHandling = MissingMemberHandling.Error,
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy(),
        },
    };

    [Pure]
    public static T Load<T>(int formatVersion, Stream stream)
        where T : IModConfig
    {
        using var reader = new StreamReader(stream);
        using var jsonReader = new JsonTextReader(reader);

        var root = JObject.Load(jsonReader, LoadSettings);

        if (!root.TryGetValue(FormatVersionKey, out var token) ||
            token.Type != JTokenType.Integer)
        {
            throw new InvalidDataException($"Missing or invalid {FormatVersionKey}.");
        }

        var version = token.Value<int>();
        if (version != formatVersion)
        {
            throw new NotSupportedException(
                $"Unsupported manifest format version {version}. " +
                $"Expected {formatVersion}.");
        }

        var serializer = JsonSerializer.Create(SerializerSettings);
        return root.ToObject<T>(serializer)
            ?? throw new JsonSerializationException(
                $"Failed to deserialize {nameof(T)}: JSON root was null.");
    }
}
