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

    public void Previous()
    {
        this.ChangeSelection(-1);
    }

    public void Next()
    {
        this.ChangeSelection(1);
    }

    protected abstract void OnSelect(Skin skin);

    private void ChangeSelection(int offset)
    {
        this.selectedIndex = (this.selectedIndex + offset + this.Skins.Count) % this.Skins.Count;
        var selectedSkin = this.Skins[this.selectedIndex];
        this.OnSelect(selectedSkin);
    }
}
