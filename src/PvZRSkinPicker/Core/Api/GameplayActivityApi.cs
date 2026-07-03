namespace PvZRSkinPicker.Api;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.TreeStateActivities;

[HarmonyPatch]
internal static class GameplayActivityApi
{
    public static GameplayActivity? Instance { get; private set; }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameplayActivity), nameof(GameplayActivity.Awake))]
    private static void Awake(GameplayActivity __instance)
    {
        Instance = __instance;
    }
}
