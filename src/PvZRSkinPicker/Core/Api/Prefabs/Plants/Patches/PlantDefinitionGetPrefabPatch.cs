namespace PvZRSkinPicker.Api.Prefabs.Plants.Patches;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Data;

using UnityEngine.AddressableAssets;

[HarmonyPatch(typeof(PlantDefinition), nameof(PlantDefinition.GetPrefab))]
internal static class PlantDefinitionGetPrefabPatch
{
    [HarmonyPrefix]
    private static bool Prefix(
        PlantDefinition __instance,
        ref AssetReferenceGameObject __result,
        bool easterEggAllowed,
        bool forceDecemberContent,
        bool forceNormalContent,
        bool forceRetroContent)
    {
        if (PlantPrefabResolver.Instance.Overrides.TryGetValue(__instance.SeedType, out var skin))
        {
            // TODO: Ignore resolver on certain conditions (params)?
            __result = skin.Prefab;
            return false;
        }

        return true;
    }
}
