namespace PvZRSkinPicker.Skins.Picker;

using Il2CppReloaded.Gameplay;

using MelonLoader;

using PvZRSkinPicker.Api;

using UnityEngine;

internal sealed class PlantSkinQuickSwap : IDisposable
{
    private const float DebounceInterval = 0.15f;

    private readonly IReadOnlyDictionary<SeedType, SkinPicker<SeedType>> pickers;

    private (float Time, int Direction) lastScroll = (float.NegativeInfinity, 0);

    private bool disposed;

    public PlantSkinQuickSwap(
        IReadOnlyDictionary<SeedType, SkinPicker<SeedType>> pickers)
    {
        this.pickers = pickers;
        MelonEvents.OnUpdate.Subscribe(this.Update);
    }

    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.disposed = true;
        MelonEvents.OnUpdate.Unsubscribe(this.Update);
    }

    private void Update()
    {
        SeedType selectedType = GameplayActivityApi.Instance?.Board?.GetSeedTypeInCursor(0) ?? SeedType.None;
        if (selectedType == SeedType.None)
        {
            return;
        }

        int scroll = Math.Sign(Input.mouseScrollDelta.y);
        if (scroll == 0)
        {
            return;
        }

        bool directionChanged = scroll != this.lastScroll.Direction;
        if (!directionChanged
            && Time.unscaledTime - this.lastScroll.Time < DebounceInterval)
        {
            return;
        }

        if (this.pickers.TryGetValue(selectedType, out var picker))
        {
            if (scroll > 0)
            {
                picker.Previous();
            }
            else
            {
                picker.Next();
            }

            this.lastScroll = (Time.unscaledTime, scroll);
        }
    }
}
