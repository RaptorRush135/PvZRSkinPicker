namespace PvZRSkinPicker.Api.Prefabs;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Data;
using Il2CppReloaded.Gameplay;

using UnityEngine.AddressableAssets;

[HarmonyPatch]
internal static class ZombiePrefabResolver
{
    private static Dictionary<ZombieType, AssetReferenceGameObject> Overrides { get; } = [];

    public static void SetOverride(ZombieType type, AssetReferenceGameObject prefab)
    {
        Overrides[type] = prefab;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ZombieDefinition), nameof(ZombieDefinition.Prefab), MethodType.Getter)]
    private static void GetPrefabPostfix(
        ZombieDefinition __instance,
        ref AssetReferenceGameObject __result)
    {
        if (Overrides.TryGetValue(__instance.ZombieType, out var @override))
        {
            __result = @override;
        }
    }
}
