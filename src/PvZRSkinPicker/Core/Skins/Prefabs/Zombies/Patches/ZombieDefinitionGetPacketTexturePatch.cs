namespace PvZRSkinPicker.Skins.Prefabs.Zombies.Patches;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Data;

using UnityEngine;

// [HarmonyPatch(typeof(ZombieDefinition), nameof(ZombieDefinition.PacketTexture), MethodType.Getter)]
internal static class ZombieDefinitionGetPacketTexturePatch
{
    [HarmonyPrefix]
    private static bool Prefix(
        ZombieDefinition __instance,
        ref Sprite __result)
    {
        return true;
    }
}
