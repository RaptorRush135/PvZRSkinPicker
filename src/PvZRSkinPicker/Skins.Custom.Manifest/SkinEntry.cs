namespace PvZRSkinPicker.Skins.Custom.Manifest;

using Newtonsoft.Json;

internal sealed record SkinEntry(
    [property: JsonRequired] string Type,
    [property: JsonRequired] string Name,
    [property: JsonRequired] Guid Id,
    [property: JsonRequired] string Directory)
{
    public override string ToString() => $"{this.Name}({this.Type})({this.Id})";
}
