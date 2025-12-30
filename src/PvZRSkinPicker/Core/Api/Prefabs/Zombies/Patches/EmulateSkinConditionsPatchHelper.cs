namespace PvZRSkinPicker.Api.Prefabs.Zombies.Patches;

using Il2CppReloaded.Gameplay;

internal static class EmulateSkinConditionsPatchHelper
{
    public static void Prefix(Zombie zombie, out EmulateSkinConditionsPatchState state)
    {
        Prefix(zombie.mZombieType, out state);
    }

    public static void Prefix(ZombieType zombieType, out EmulateSkinConditionsPatchState state)
    {
        bool needsClear = ZombiePrefabResolver.EmulateSkinConditions(zombieType);
        state = new EmulateSkinConditionsPatchState(needsClear);
    }

    public static void Postfix(EmulateSkinConditionsPatchState state)
    {
        if (state.NeedsClear)
        {
            ZombiePrefabResolver.ClearGameplayOverrides();
        }
    }
}
