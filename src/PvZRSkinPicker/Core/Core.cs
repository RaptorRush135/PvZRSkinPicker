namespace PvZRSkinPicker;

using Il2CppReloaded.Data;
using Il2CppReloaded.DataModels;

using MelonLoader;

using PvZRSkinPicker.Almanac;
using PvZRSkinPicker.Almanac.UI;
using PvZRSkinPicker.Api.Context;
using PvZRSkinPicker.Data;
using PvZRSkinPicker.Extensions;
using PvZRSkinPicker.Skins.Picker;
using PvZRSkinPicker.Skins.Prefabs;
using PvZRSkinPicker.Skins.Prefabs.Plants;
using PvZRSkinPicker.Skins.Prefabs.Zombies;

public sealed class Core : MelonMod
{
    public override void OnLateInitializeMelon()
    {
        try
        {
            ModContextApi.Initialize();
        }
        catch (Exception ex)
        {
            Melon<Core>.Logger.Error("Failed to initialize skin picker", ex);
            this.Unregister();
            return;
        }

        ModContextApi.Ready.Subscribe(Ready);
    }

    public override void OnDeinitializeMelon()
    {
        ModContextApi.Dispose();
    }

    private static void Ready(ModContext context)
    {
        SetupSkinPicker(
            AlmanacEntryType.Plant,
            context.Almanac.m_plantsModel,
            context.DataService.PlantDefinitions.AsEnumerable()
                .Select(d => new PlantSkinDataDefinition(d, context.PlatformService)),
            PlantSkinOverrideResolver.Instance);

        SetupSkinPicker(
            AlmanacEntryType.Zombie,
            context.Almanac.m_zombiesModel,
            context.DataService.ZombieDefinitions.AsEnumerable()
                .Select(d => new ZombieSkinDataDefinition(d, context.PlatformService)),
            ZombieSkinOverrideResolver.Instance);
    }

    private static void SetupSkinPicker<T>(
        AlmanacEntryType type,
        AlmanacEntriesModel entriesModel,
        IEnumerable<ISkinDataDefinition<T>> definitions,
        SkinOverrideResolver<T> prefabResolver)
        where T : struct, Enum
    {
        var button = SkinSwapUI.CreateButton(type);

        var selection = new AlmanacSelection<T>(entriesModel.m_selectedModel);

        var controller = new SkinPickerController<T>(selection, definitions, prefabResolver.SetOverride);

        controller.ApplySelections();
        controller.Bind(button);
    }
}
