namespace PvZRSkinPicker.Configuration;

using System.Diagnostics.Contracts;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

internal static class ModConfig
{
    public const string FormatVersionKey = "format_version";

    private static readonly JsonLoadSettings LoadSettings = new()
    {
        CommentHandling = CommentHandling.Ignore,
        DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error,
    };

    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        MissingMemberHandling = MissingMemberHandling.Ignore,
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy(),
        },
    };

    private static JsonSerializer Serializer => field ??= JsonSerializer.Create(SerializerSettings);

    [Pure]
    public static T Load<T>(int formatVersion, Stream stream, Action<string>? logger = null)
        where T : IModConfig
    {
        using var reader = new StreamReader(stream);
        using var jsonReader = new JsonTextReader(reader);

        var root = JObject.Load(jsonReader, LoadSettings);

        if (!root.TryGetValue(FormatVersionKey, out var token) ||
            token.Type != JTokenType.Integer)
        {
            // Default to current format version if missing
            return root.ToObject<T>(Serializer)
                ?? throw new JsonSerializationException(
                    $"Failed to deserialize {nameof(T)}: JSON root was null.");
        }

        var version = token.Value<int>();
        if (version != formatVersion)
        {
            // Log a warning instead of throwing, as per developer request
            logger?.Invoke(
                $"Manifest format version mismatch (found {version}, expected {formatVersion}). " +
                "Continuing anyway as requested by developer.");
        }

        return root.ToObject<T>(Serializer)
            ?? throw new JsonSerializationException(
                $"Failed to deserialize {nameof(T)}: JSON root was null.");
    }

    public static void Write<T>(T config, Stream stream)
        where T : IModConfig
    {
        using var streamWriter = new StreamWriter(stream);
        using var jsonWriter = new JsonTextWriter(streamWriter)
        {
            Formatting = Formatting.Indented,
        };

        Serializer.Serialize(jsonWriter, config);
    }
}
