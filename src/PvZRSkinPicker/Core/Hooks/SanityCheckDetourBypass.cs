namespace PvZRSkinPicker.Hooks;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using System.Reflection;

using HarmonyLib;

using MelonLoader;

internal sealed class SanityCheckDetourBypass : IDisposable
{
    private static readonly MethodInfo? Target = AccessTools.Method(
       "MelonLoader.CoreClrUtils.CoreClrDelegateFixer:SanityCheckDetour");

    private static readonly HarmonyMethod PatchMethod = AccessTools.Method(
        typeof(SanityCheckDetourBypass), nameof(CheckBypassPatch))
            .ToNewHarmonyMethod();

    private readonly Harmony harmony;

    public SanityCheckDetourBypass(Harmony harmony)
    {
        ArgumentNullException.ThrowIfNull(harmony);

        this.harmony = harmony;

        if (Target == null)
        {
            Melon<Core>.Logger.Warning("Failed to locate SanityCheckDetour");
            return;
        }

        PatchShieldBypass.ExecuteWithBypass(
            () => this.harmony.Patch(Target, prefix: PatchMethod));
    }

    public void Dispose()
    {
        if (Target != null)
        {
            PatchShieldBypass.ExecuteWithBypass(
                () => this.harmony.Unpatch(Target, PatchMethod.method));
        }
    }

    private static bool CheckBypassPatch(ref bool __result)
    {
        __result = true;
        return false;
    }
}
