namespace PvZRSkinPicker.Api.Prefabs;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Data;
using Il2CppReloaded.Gameplay;

using PvZRSkinPicker.Skins;

using UnityEngine.AddressableAssets;

[HarmonyPatch]
internal static class PlantPrefabResolver
{
    private static Dictionary<SeedType, Skin> Overrides { get; } = [];

    public static void SetOverride(SeedType type, Skin skin)
    {
        Overrides[type] = skin;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlantDefinition), nameof(PlantDefinition.GetPrefab))]
    private static void GetPrefabPostfix(
        PlantDefinition __instance,
        ref AssetReferenceGameObject __result)
    {
        if (Overrides.TryGetValue(__instance.SeedType, out var skin))
        {
            __result = skin.Prefab;
        }
    }
}
