namespace PvZRSkinPicker.Skins.Prefabs;

using HarmonyLib;

using PvZRSkinPicker.Skins.Prefabs.Plants;
using PvZRSkinPicker.Skins.Prefabs.Zombies;

using SolarApi.Collections;
using SolarApi.MelonLoader;

public sealed class SkinOverrideResolverManager(
    Harmony harmony,
    DisposeGroup disposeGroup)
{
    public void Initialize()
    {
        using var scope = new SanityCheckDetourBypass(harmony);

        disposeGroup.Collect(PlantSkinOverrideResolver.Initialize());
        disposeGroup.Collect(ZombieSkinOverrideResolver.Initialize());
    }
}
