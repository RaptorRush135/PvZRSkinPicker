namespace PvZRSkinPicker.Skins.Custom.Manifest;

using System.Diagnostics.Contracts;

using Newtonsoft.Json;

using PvZRSkinPicker.Configuration;

internal sealed record SkinEntry(
    [property: JsonRequired] string Type,
    [property: JsonRequired] string Name,
    [property: JsonRequired] Guid Id,
    [property: JsonRequired] string Directory,
    bool Pixelated)
{
    public override string ToString() => $"{this.Name}({this.Type})({this.Id})";

    [Pure]
    public string? Validate()
    {
        if (ModConfigValidator.ValidateString(
            this.Type,
            nameof(this.Type),
            ch => (ch is '-') || (char.IsAscii(ch) && char.IsLetter(ch)),
            40) is { } typeError)
        {
            return typeError;
        }

        if (ModConfigValidator.ValidateString(
            this.Name,
            nameof(this.Name),
            ModConfigValidator.IsPrintableAscii,
            30) is { } nameError)
        {
            return nameError;
        }

        if (this.Id == Guid.Empty)
        {
            return $"Skin '{nameof(this.Id)}' can not be a empty Guid";
        }

        if (ModConfigValidator.ValidateString(
            this.Directory,
            nameof(this.Directory),
            ch => (ch is '-' or '_') || (char.IsAscii(ch) && char.IsLetterOrDigit(ch)),
            30) is { } directoryError)
        {
            return directoryError;
        }

        return null;
    }
}
