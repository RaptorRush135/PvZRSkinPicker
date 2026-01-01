namespace PvZRSkinPicker.Api.Prefabs.Patches;

using Il2CppReloaded.Gameplay;

internal sealed class EmulateSkinConditionsPatcher<TClass, T>(
    PrefabResolver<T> prefabResolver,
    Func<TClass, T> typeGetter)
    where TClass : ReloadedObject
    where T : struct, Enum
{
    public void Prefix(TClass instance, out EmulateSkinConditionsPatchState state)
    {
        this.Prefix(typeGetter(instance), out state);
    }

    public void Prefix(T type, out EmulateSkinConditionsPatchState state)
    {
        bool needsClear = prefabResolver.EmulateSkinConditions(type);
        state = new EmulateSkinConditionsPatchState(needsClear);
    }

    public void Postfix(EmulateSkinConditionsPatchState state)
    {
        if (state.NeedsClear)
        {
            prefabResolver.ClearSkinConditions();
        }
    }
}
