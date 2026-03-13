namespace PvZRSkinPicker.Skins.Custom.Manifest;

using System.Diagnostics.Contracts;

using Newtonsoft.Json;

internal sealed record SkinEntry(
    [property: JsonRequired] string Type,
    [property: JsonRequired] string Name,
    [property: JsonRequired] Guid Id,
    [property: JsonRequired] string Directory)
{
    public override string ToString() => $"{this.Name}({this.Type})({this.Id})";

    [Pure]
    public string? Validate()
    {
        if (ValidateString(
            this.Name,
            nameof(this.Name),
            IsPrintableAscii,
            30) is { } nameError)
        {
            return nameError;
        }

        if (ValidateString(
            this.Directory,
            nameof(this.Directory),
            ch => (ch is '-' or '_') || (char.IsAscii(ch) && char.IsLetterOrDigit(ch)),
            30) is { } directoryError)
        {
            return directoryError;
        }

        if (this.Id == Guid.Empty)
        {
            return $"Skin '{nameof(this.Id)}' can not be a empty Guid";
        }

        return null;
    }

    [Pure]
    private static string? ValidateString(
        string @string,
        string stringName,
        Predicate<char> validator,
        int maxLength)
    {
        if (ValidateStringLength(@string, stringName, maxLength) is { } lengthError)
        {
            return lengthError;
        }

        char invalid = @string.FirstOrDefault(ch => !validator(ch));

        return invalid != default
            ? $"Invalid character on '{stringName}' ({GetCharDisplay(invalid)})"
            : null;

        static string GetCharDisplay(char ch)
            => IsPrintableAscii(ch) ? $"{ch}" : $"U+{(int)ch:X4}";
    }

    [Pure]
    private static string? ValidateStringLength(
        string @string,
        string stringName,
        int maxLength)
    {
        if (string.IsNullOrEmpty(@string))
        {
            return $"{stringName} must not be empty";
        }

        if (@string.Length > maxLength)
        {
            return $"{stringName} length must not be greater than {maxLength}";
        }

        return null;
    }

    [Pure]
    private static bool IsPrintableAscii(char ch) => ch >= ' ' && ch <= '~';
}
