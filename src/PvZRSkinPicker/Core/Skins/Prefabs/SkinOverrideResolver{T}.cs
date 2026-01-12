namespace PvZRSkinPicker.Skins.Prefabs;

using System.Diagnostics.CodeAnalysis;

using MelonLoader;

using PvZRSkinPicker.Api;

using PvZRSkinPicker.Skins;

internal abstract class SkinOverrideResolver<T>
    where T : struct, Enum
{
    private readonly SpawnContextContainer<T> currentContext = new(Melon<Core>.Logger);

    private readonly Dictionary<T, Skin> overrides = [];

    public void SetOverride(T type, Skin skin)
    {
        this.overrides[type] = skin;
    }

    public bool TryGetContextOverride(T type, [MaybeNullWhen(false)] out Skin skin)
    {
        if (this.currentContext.Get() is not { } context)
        {
            return this.overrides.TryGetValue(type, out skin);
        }

        if (!EqualityComparer<T>.Default.Equals(context.Type, type))
        {
            this.currentContext.Warning($"type mismatch ({context.Type} / {type})");
            skin = null;
            return false;
        }

        return this.TryGetOverride(context, out skin);
    }

    public void EmulateSkinConditions(SpawnContext<T> context)
    {
        this.currentContext.Set(context);

        if (this.TryGetOverride(context, out var skin))
        {
            SkinConditionEmulator.ApplyGameplayOverridesForSkinType(skin.Type);
        }
    }

    public virtual void ClearSkinConditions()
    {
        this.currentContext.Clear();
        GameplayServiceApi.SetOverrides(null);
    }

    protected virtual bool IsSkinCompatible(SkinType skinType, SpawnContext<T> context) => true;

    private bool TryGetOverride(SpawnContext<T> context, [MaybeNullWhen(false)] out Skin skin)
    {
        if (!this.overrides.TryGetValue(context.Type, out skin))
        {
            return false;
        }

        if (!this.IsSkinCompatible(skin.Type, context))
        {
            skin = null;
            return false;
        }

        return true;
    }
}
