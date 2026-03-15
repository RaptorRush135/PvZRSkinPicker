namespace PvZRSkinPicker.Hooks;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable IDE0051 // Remove unused private member

using System.Reflection;

using HarmonyLib;

using MelonLoader;

[HarmonyPatch]
internal static class PatchShieldBypass
{
    public static bool Bypass { get; set; }

    [HarmonyTargetMethod]
    private static MethodBase TargetMethod()
    {
        return AccessTools.Method(
            "System.Reflection.RuntimeAssembly:GetCustomAttributes",
            [typeof(Type), typeof(bool)]);
    }

    [HarmonyPrefix]
    private static bool GetCustomAttributesPatch(Type attributeType, ref object[] __result)
    {
        if (!Bypass)
        {
            return true;
        }

        if (attributeType == typeof(PatchShield))
        {
            __result = [];
            return false;
        }

        return true;
    }
}
