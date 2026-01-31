namespace PvZRSkinPicker.Skins.Custom.Manifest;

using Newtonsoft.Json;

internal record SkinCatalog(
    [property: JsonRequired]
    IReadOnlyList<SkinEntry> Plants);
