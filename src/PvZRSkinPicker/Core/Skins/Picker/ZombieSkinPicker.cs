namespace PvZRSkinPicker.Skins.Picker;

using Il2CppReloaded.Gameplay;

using PvZRSkinPicker.Api.Prefabs.Zombies;

internal sealed class ZombieSkinPicker : SkinPicker<ZombieType>
{
    protected override void OnSelect(Skin skin)
    {
        ZombiePrefabResolver.SetOverride(this.Type, skin);
    }
}
