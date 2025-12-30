namespace PvZRSkinPicker;

using Il2CppReloaded.Data;
using Il2CppReloaded.DataModels;
using Il2CppReloaded.Gameplay;

using MelonLoader;

using PvZRSkinPicker.Almanac;
using PvZRSkinPicker.Almanac.UI;
using PvZRSkinPicker.Api.Context;
using PvZRSkinPicker.Data;
using PvZRSkinPicker.Extensions;
using PvZRSkinPicker.Skins.Picker;

public sealed class Core : MelonMod
{
    public override void OnLateInitializeMelon()
    {
        ModContextApi.Ready += Ready;
    }

    public override void OnDeinitializeMelon()
    {
        ModContextApi.Dispose();
    }

    private static void Ready(ModContext context)
    {
        SetupSkinPicker<SeedType, PlantSkinPicker>(
            AlmanacEntryType.Plant,
            context.Almanac.m_plantsModel,
            context.DataService.PlantDefinitions.AsEnumerable()
                .Select(d => new PlantSkinDataDefinition(d, context.PlatformService)));

        SetupSkinPicker<ZombieType, ZombieSkinPicker>(
            AlmanacEntryType.Zombie,
            context.Almanac.m_zombiesModel,
            context.DataService.ZombieDefinitions.AsEnumerable()
                .Select(d => new ZombieSkinDataDefinition(d, context.PlatformService)));
    }

    private static void SetupSkinPicker<T, TPicker>(
        AlmanacEntryType type,
        AlmanacEntriesModel entriesModel,
        IEnumerable<ISkinDataDefinition<T>> definitions)
        where T : struct, Enum
        where TPicker : SkinPicker<T>, new()
    {
        var button = SkinSwapUI.CreateButton(type);

        var selection = new AlmanacSelection<T>(entriesModel.m_selectedModel);

        var controller = new SkinPickerController<T, TPicker>(
            selection,
            definitions);

        controller.Bind(button);
    }
}
