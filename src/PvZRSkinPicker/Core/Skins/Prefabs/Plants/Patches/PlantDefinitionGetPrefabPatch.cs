namespace PvZRSkinPicker.Skins.Prefabs.Plants.Patches;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Data;

using PvZRSkinPicker.Skins.Prefabs.Plants;

using UnityEngine.AddressableAssets;

[HarmonyPatch(typeof(PlantDefinition), nameof(PlantDefinition.GetPrefab))]
internal static class PlantDefinitionGetPrefabPatch
{
    [HarmonyPrefix]
    private static bool Prefix(
        PlantDefinition __instance,
        ref AssetReferenceGameObject __result,
        bool forceDecemberContent,
        bool forceRetroContent)
    {
        if (forceDecemberContent || forceRetroContent)
        {
            return true;
        }

        if (PlantPrefabResolver.Instance.Overrides.TryGetValue(__instance.SeedType, out var skin))
        {
            __result = skin.Prefab;
            return false;
        }

        return true;
    }
}
