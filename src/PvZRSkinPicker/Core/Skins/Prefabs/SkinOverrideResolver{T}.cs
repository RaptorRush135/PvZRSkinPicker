namespace PvZRSkinPicker.Skins.Prefabs;

using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Logging;

using PvZRSkinPicker.Almanac.SeedPackets;
using PvZRSkinPicker.Api;
using PvZRSkinPicker.Skins;

internal abstract class SkinOverrideResolver<T>(
    ILogger<SkinOverrideResolver<T>> logger,
    SpawnContextContainer<T> currentContext)
    where T : struct, Enum
{
    private readonly Dictionary<T, Skin> overrides = [];

    protected abstract PacketThumbnailLookup<T>? PacketThumbnailLookup { get; }

    public void SetOverride(T type, Skin skin)
    {
        this.overrides[type] = skin;
        foreach (var extraOverride in this.GetExtraTypeOverrides(type))
        {
            this.overrides[extraOverride] = skin;
        }

        if (this.PacketThumbnailLookup != null)
        {
            foreach (var thumbnail in this.PacketThumbnailLookup.GetThumbnails(type))
            {
                thumbnail.Reference = skin.Image;
            }
        }
    }

    public virtual ReadOnlySpan<T> GetExtraTypeOverrides(T type) => [];

    public bool TryGetContextOverride(T type, [MaybeNullWhen(false)] out Skin skin)
    {
        if (currentContext.Get() is not { } context)
        {
            return this.overrides.TryGetValue(type, out skin);
        }

        if (!EqualityComparer<T>.Default.Equals(context.Type, type))
        {
            currentContext.Warning($"type mismatch ({context.Type} / {type})");
            skin = null;
            return false;
        }

        return this.TryGetOverride(context, out skin);
    }

    public void EmulateSkinConditions(SpawnContext<T> context)
    {
        currentContext.Set(context);

        if (this.TryGetOverride(context, out var skin))
        {
            ApplyGameplayOverridesForSkinType(skin.Id.Type);
        }
    }

    public void OnForcedDecember()
    {
        if (currentContext.Get() == null)
        {
            logger.LogWarning(
                $"{nameof(this.OnForcedDecember)} was called but no spawn context is active");

            return;
        }

        GameplayServiceApi.RetroContentActiveOverride = false;
    }

    public void ClearSkinConditions()
    {
        currentContext.Clear();
        GameplayServiceApi.SetOverrides(null);
    }

    protected virtual bool IsSkinCompatible(SkinType skinType, SpawnContext<T> context) => true;

    private static void ApplyGameplayOverridesForSkinType(SkinType skinType)
    {
        GameplayServiceApi.SetOverrides(false);

        switch (skinType)
        {
            case SkinType.PreOrderPlant:
                GameplayServiceApi.PreOrderContentActiveOverride = true;
                break;
            case SkinType.RetroZombie:
                GameplayServiceApi.RetroContentActiveOverride = true;
                break;
            case SkinType.PlatformZombie:
                GameplayServiceApi.PlatformContentActiveOverride = true;
                break;
            case SkinType.China:
                GameplayServiceApi.ChinaModeActiveOverride = true;
                break;
        }
    }

    private bool TryGetOverride(SpawnContext<T> context, [MaybeNullWhen(false)] out Skin skin)
    {
        if (!this.overrides.TryGetValue(context.Type, out skin))
        {
            return false;
        }

        if (!this.IsSkinCompatible(skin.Id.Type, context))
        {
            skin = null;
            return false;
        }

        return true;
    }
}
