namespace PvZRSkinPicker;

using System.Collections.Immutable;

using Il2CppReloaded.Data;
using Il2CppReloaded.DataModels;
using Il2CppReloaded.Gameplay;

using MelonLoader;

using PvZRSkinPicker.Almanac;
using PvZRSkinPicker.Almanac.UI;
using PvZRSkinPicker.Api.Context;
using PvZRSkinPicker.Data;
using PvZRSkinPicker.Environment;
using PvZRSkinPicker.Extensions;
using PvZRSkinPicker.Skins;
using PvZRSkinPicker.Skins.Custom;
using PvZRSkinPicker.Skins.Picker;
using PvZRSkinPicker.Skins.Picker.Selection;
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
        var skinLocator = new SkinLocator(context.PlatformService, context.Localizer);

        var customSkinLoader = new CustomSkinLoader(Melon<Core>.Logger, context.DataService);

        var skinSelectionPersistence = new SkinSelectionPersistence(
            ModEnvironment.ModDataDirectory.GetFile("selections.json"));

        SkinSelections skinSelections = skinSelectionPersistence.TryReadSelections();

        var plantPickerController = SetupSkinPicker(
            AlmanacEntryType.Plant,
            context.Almanac.m_plantsModel,
            context.DataService.PlantDefinitions.AsEnumerable()
                .Select(d => new PlantSkinDataDefinition(d, skinLocator)),
            customSkinLoader.GetPlantSkins(),
            PlantSkinOverrideResolver.Instance,
            skinSelections.Plants);

        var zombiePickerController = SetupSkinPicker(
            AlmanacEntryType.Zombie,
            context.Almanac.m_zombiesModel,
            context.DataService.ZombieDefinitions.AsEnumerable()
                .Select(d => new ZombieSkinDataDefinition(d, skinLocator)),
            ImmutableDictionary<ZombieType, IReadOnlyList<Skin>>.Empty,
            ZombieSkinOverrideResolver.Instance,
            skinSelections.Zombies);

        skinSelectionPersistence.BindControllers(plantPickerController, zombiePickerController);
    }

    private static SkinPickerController<T> SetupSkinPicker<T>(
        AlmanacEntryType type,
        AlmanacEntriesModel entriesModel,
        IEnumerable<ISkinDataDefinition<T>> definitions,
        IReadOnlyDictionary<T, IReadOnlyList<Skin>> extraSkins,
        SkinOverrideResolver<T> skinOverrideResolver,
        SkinSelectionSet<T> selectionSet)
        where T : struct, Enum
    {
        var button = SkinSwapUI.CreateButton(type);

        var selection = AlmanacSelection<T>.Create(type, entriesModel.m_selectedModel);

        var controller = new SkinPickerController<T>(
            selection,
            definitions,
            extraSkins,
            onSelect: skinOverrideResolver.SetOverride);

        controller.ApplySelections(selectionSet);
        controller.Bind(button);

        return controller;
    }
}
