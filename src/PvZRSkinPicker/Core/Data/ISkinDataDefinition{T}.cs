namespace PvZRSkinPicker.Data;

using PvZRSkinPicker.Skins;

internal interface ISkinDataDefinition<T>
{
    T Type { get; }

    IEnumerable<Skin> GetSkins();
}
