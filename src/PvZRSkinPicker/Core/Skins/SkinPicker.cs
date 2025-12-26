namespace PvZRSkinPicker.Skins;

using System.Diagnostics.Contracts;

internal abstract class SkinPicker<T>
    where T : struct, Enum
{
    private int selectedIndex;

    public T Type { get; private init; }

    public IReadOnlyList<Skin> Skins { get; private init; } = null!;

    [Pure]
    public static TPicker? TryCreate<TPicker>(T type, IEnumerable<Skin> skins)
        where TPicker : SkinPicker<T>, new()
    {
        var skinArray = skins.ToArray();
        return skinArray.Length <= 1 ? null : new TPicker()
        {
            Type = type,
            Skins = skinArray,
        };
    }

    public void Next()
    {
        this.selectedIndex = (this.selectedIndex + 1) % this.Skins.Count;
        var selectedSkin = this.Skins[this.selectedIndex];
        this.OnSelect(selectedSkin);
    }

    protected abstract void OnSelect(Skin skin);
}
