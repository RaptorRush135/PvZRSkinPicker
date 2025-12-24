namespace PvZRSkinPicker.Skins;

using Il2CppReloaded.Gameplay;

using PvZRSkinPicker.Api.Prefabs;

internal sealed class PlantSkinPicker : SkinPicker<SeedType>
{
    protected override void OnSelect(Skin skin)
    {
        PlantPrefabResolver.SetOverride(this.Type, skin.Prefab);
    }
}
