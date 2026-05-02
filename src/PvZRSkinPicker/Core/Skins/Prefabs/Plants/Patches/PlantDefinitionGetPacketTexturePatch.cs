namespace PvZRSkinPicker.Skins.Prefabs.Plants.Patches;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Data;

using UnityEngine;

// [HarmonyPatch(typeof(PlantDefinition), nameof(PlantDefinition.PacketTexture), MethodType.Getter)]
internal static class PlantDefinitionGetPacketTexturePatch
{
    [HarmonyPrefix]
    private static bool Prefix(
        PlantDefinition __instance,
        ref Sprite __result)
    {
        return true;
    }
}
