namespace PvZRSkinPicker.Skins.Prefabs;

using Il2CppReloaded.Gameplay;

// TODO: Delete & refactor?
internal sealed class EmulateSkinConditionsPatcher<TClass, T>(
    SkinOverrideResolver<T> skinOverrideResolver,
    Func<TClass, SpawnContext<T>> contextGetter)
    where TClass : ReloadedObject
    where T : struct, Enum
{
    public void Prefix(TClass instance)
    {
        this.Prefix(contextGetter(instance));
    }

    public void Prefix(SpawnContext<T> context)
    {
        skinOverrideResolver.EmulateSkinConditions(context);
    }

    public void Postfix()
    {
        skinOverrideResolver.ClearSkinConditions();
    }
}
