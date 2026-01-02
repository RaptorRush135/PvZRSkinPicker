namespace PvZRSkinPicker.Skins.Prefabs.Zombies.Patches;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable IDE0051 // Remove unused private member

using HarmonyLib;

using Il2CppReloaded.Data;

using PvZRSkinPicker.Skins.Prefabs.Zombies;

using UnityEngine.AddressableAssets;

[HarmonyPatch(typeof(ZombieDefinition), nameof(ZombieDefinition.Prefab), MethodType.Getter)]
internal static class ZombieDefinitionGetPrefabPatch
{
    [HarmonyPrefix]
    private static bool Prefix(
        ZombieDefinition __instance,
        ref AssetReferenceGameObject __result)
    {
        if (ZombiePrefabResolver.Instance.Overrides.TryGetValue(__instance.ZombieType, out var skin))
        {
            __result = skin.Prefab;
            return false;
        }

        return true;
    }
}
