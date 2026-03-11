namespace PvZRSkinPicker.Skins.Custom.Manifest;

using System.Diagnostics.Contracts;

internal sealed record SkinCatalog
{
    public IReadOnlyList<SkinEntry> Plants { get; init; } = [];

    [Pure]
    public string? Validate()
    {
        if (ValidateSkins(this.Plants, "Plant") is { } plantsError)
        {
            return plantsError;
        }

        return null;
    }

    [Pure]
    private static string? ValidateSkins(IReadOnlyList<SkinEntry> skins, string name)
    {
        foreach (var skin in skins)
        {
            if (skin.Validate() is { } skinError)
            {
                return skinError;
            }
        }

        if (ContainsDuplicates(skins.Select(p => p.Id)))
        {
            return $"{name} ids must be unique";
        }

        return null;
    }

    [Pure]
    private static bool ContainsDuplicates<T>(IEnumerable<T> source)
    {
        var valueSet = new HashSet<T>();
        return source.Any(item => !valueSet.Add(item));
    }
}
