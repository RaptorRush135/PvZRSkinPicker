namespace PvZRSkinPicker.Api.Prefabs;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Data;
using Il2CppReloaded.Gameplay;

using UnityEngine.AddressableAssets;

[HarmonyPatch]
internal static class PlantPrefabResolver
{
    private static Dictionary<SeedType, AssetReferenceGameObject> Overrides { get; } = [];

    public static void SetOverride(SeedType type, AssetReferenceGameObject prefab)
    {
        Overrides[type] = prefab;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlantDefinition), nameof(PlantDefinition.GetPrefab))]
    private static void GetPrefabPostfix(
        PlantDefinition __instance,
        ref AssetReferenceGameObject __result)
    {
        if (Overrides.TryGetValue(__instance.SeedType, out var @override))
        {
            __result = @override;
        }
    }
}
