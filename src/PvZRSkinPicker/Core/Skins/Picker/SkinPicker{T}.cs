namespace PvZRSkinPicker.Skins.Picker;

using System.Diagnostics.Contracts;

using Il2CppReloaded.Services;

using PvZRSkinPicker.Api;
using PvZRSkinPicker.Data;
using PvZRSkinPicker.Skins;

internal abstract class SkinPicker<T>
    where T : struct, Enum
{
    private int selectedIndex;

    public T Type { get; private init; }

    public IReadOnlyList<Skin> Skins { get; private init; } = null!;

    [Pure]
    public static TPicker? TryCreate<TPicker>(ISkinDataDefinition<T> definition)
        where TPicker : SkinPicker<T>, new()
    {
        return TryCreate<TPicker>(definition.Type, definition.GetSkins());
    }

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
        AudioServiceApi.PlayWithRandomPitch(FoleyType.LimbsPop);
    }

    protected abstract void OnSelect(Skin skin);
}
