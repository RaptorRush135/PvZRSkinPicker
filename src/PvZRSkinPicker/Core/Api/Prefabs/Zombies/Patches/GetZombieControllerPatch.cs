namespace PvZRSkinPicker.Api.Prefabs.Zombies.Patches;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded;
using Il2CppReloaded.Data;

using PvZRSkinPicker.Skins;

[HarmonyPatch(typeof(GameplayPooler), nameof(GameplayPooler.GetZombieController))]
internal static class GetZombieControllerPatch
{
    [HarmonyPrefix]
    private static void Prefix(
        ZombieDefinition zombieDefinition,
        ref bool easterEggAllowed,
        ref bool forceDecember,
        ref bool forceNormal,
        out GetZombieControllerPatchState __state)
    {
        /*
         * Zombie.ZombieInitialize:
         *   easterEggAllowed = true
         *   forceDecember    = false
         *   forceNormal      = false
         *
         * Zombie.Deserialize:
         *   easterEggAllowed = false
         *   forceDecember    = saveFile
         *   forceNormal      = !forceDecember
         */
        __state = new(zombieDefinition.EasterEggChance);

        if (forceDecember)
        {
            return;
        }

        if (!ZombiePrefabResolver.Overrides.TryGetValue(zombieDefinition.ZombieType, out var skin))
        {
            return;
        }

        SkinType skinType = skin.Type;

        if (skinType == SkinType.Normal)
        {
            forceNormal = true;
            return;
        }

        forceNormal = false;

        if (skinType == SkinType.EasterEgg)
        {
            easterEggAllowed = true;
            zombieDefinition.m_easterEggChance100 = 100;
            return;
        }

        easterEggAllowed = false;

        if (skinType == SkinType.December)
        {
            forceDecember = true;
        }
    }

    [HarmonyFinalizer]
    private static void Finalizer(
        ZombieDefinition zombieDefinition,
        GetZombieControllerPatchState __state)
    {
        zombieDefinition.m_easterEggChance100 = __state.OriginalEasterEggChance;
    }

    private readonly record struct GetZombieControllerPatchState(
        float OriginalEasterEggChance);
}
