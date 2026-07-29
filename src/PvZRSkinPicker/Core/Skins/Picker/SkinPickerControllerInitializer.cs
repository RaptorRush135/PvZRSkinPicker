namespace PvZRSkinPicker.Skins.Picker;

using Il2CppReloaded.Data;
using Il2CppReloaded.DataModels;
using Il2CppReloaded.Services;

using PvZRSkinPicker.Almanac;
using PvZRSkinPicker.Almanac.UI;
using PvZRSkinPicker.Api;
using PvZRSkinPicker.Data;
using PvZRSkinPicker.Skins.Custom;
using PvZRSkinPicker.Skins.Picker.Selection;
using PvZRSkinPicker.Skins.Prefabs;
using PvZRSkinPicker.Skins.Prefabs.Plants;
using PvZRSkinPicker.Skins.Prefabs.Zombies;

using SolarApi.Il2Cpp.Extensions;

internal sealed class SkinPickerControllerInitializer(
    AlmanacModel almanac,
    IDataService dataService,
    SkinLocator skinLocator)
{
    public SkinPickerControllerPair Create(SkinSelections selections, CustomSkinSet customSkins)
    {
        var plantPickerController = SetupSkinPicker(
            AlmanacEntryType.Plant,
            almanac.m_plantsModel,
            dataService.PlantDefinitions.AsEnumerable()
                .Select(d => new PlantSkinDataDefinition(d, skinLocator)),
            customSkins.Plants,
            PlantSkinOverrideResolver.Instance,
            selections.Plants);

        var zombiePickerController = SetupSkinPicker(
            AlmanacEntryType.Zombie,
            almanac.m_zombiesModel,
            dataService.ZombieDefinitions.AsEnumerable()
                .Select(d => new ZombieSkinDataDefinition(d, skinLocator)),
            customSkins.Zombies,
            ZombieSkinOverrideResolver.Instance,
            selections.Zombies);

        return new(plantPickerController, zombiePickerController);
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

        bool firstOpen = true;
        AlmanacApi.OnAlmanacOpened.Subscribe(openType =>
        {
            if (openType != type)
            {
                return;
            }

            controller.RefreshName(overrideUntilNextNameSet: firstOpen);
            firstOpen = false;
        });

        return controller;
    }
}
