namespace PvZRSkinPicker.Skins.Custom.Manifest;

internal sealed record SkinCatalog
{
    public IReadOnlyList<SkinEntry> Plants { get; init; } = [];
}
