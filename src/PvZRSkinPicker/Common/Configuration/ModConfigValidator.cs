namespace PvZRSkinPicker.Configuration;

using System.Diagnostics.Contracts;

internal static class ModConfigValidator
{
    [Pure]
    public static bool IsPrintableAscii(char ch) => ch >= ' ' && ch <= '~';

    [Pure]
    public static string? ValidateStrings(
        IEnumerable<string> strings,
        string stringsName,
        Predicate<char> validator,
        int maxLength)
    {
        foreach (var (index, str) in strings.Index())
        {
            if (ValidateString(str, $"{stringsName}:{index}", validator, maxLength) is { } error)
            {
                return error;
            }
        }

        return null;
    }

    [Pure]
    public static string? ValidateString(
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
}
