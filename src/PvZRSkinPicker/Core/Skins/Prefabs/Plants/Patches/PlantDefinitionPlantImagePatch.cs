namespace PvZRSkinPicker.Skins.Prefabs.Plants.Patches;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable IDE0051 // Remove unused private member

using HarmonyLib;

using Il2CppReloaded.Data;

using UnityEngine.AddressableAssets;

[HarmonyPatch(typeof(PlantDefinition), nameof(PlantDefinition.PlantImage), MethodType.Getter)]
internal static class PlantDefinitionPlantImagePatch
{
    [HarmonyPrefix]
    private static bool Prefix(
        PlantDefinition __instance,
        ref AssetReferenceSprite __result)
    {
        if (PlantSkinOverrideResolver.Instance.TryGetContextOverride(__instance.SeedType, out var skin)
            && skin.Image != null)
        {
            __result = skin.Image;
            return false;
        }

        return true;
    }
}
