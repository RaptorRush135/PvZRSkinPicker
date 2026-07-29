namespace PvZRSkinPicker;

using System.Collections.Immutable;

using HarmonyLib;

using Il2CppReloaded.Data;
using Il2CppReloaded.DataModels;
using Il2CppReloaded.Gameplay;

using MelonLoader;

using PvZRSkinPicker.Almanac;
using PvZRSkinPicker.Almanac.UI;
using PvZRSkinPicker.Api;
using PvZRSkinPicker.Data;
using PvZRSkinPicker.Environment;
using PvZRSkinPicker.Extensions;
using PvZRSkinPicker.Hooks;
using PvZRSkinPicker.Metadata;
using PvZRSkinPicker.Skins;
using PvZRSkinPicker.Skins.Custom;
using PvZRSkinPicker.Skins.Picker;
using PvZRSkinPicker.Skins.Picker.Selection;
using PvZRSkinPicker.Skins.Prefabs;
using PvZRSkinPicker.Skins.Prefabs.Plants;
using PvZRSkinPicker.Skins.Prefabs.Zombies;

using SolarApi;
using SolarApi.Il2Cpp.Extensions;
using SolarApi.Unity.Resources;

internal sealed class SkinPickerMod(
    ModContext context,
    SkinLocator skinLocator,
    Harmony harmony)
    : SolarMod
{
    private readonly HookStore hookStore = new();

    private IDisposable? quickSwap;

    protected override void OnInitialize()
    {
        using (var scope = new SanityCheckDetourBypass(harmony))
        {
            this.hookStore.Add(PlantSkinOverrideResolver.Initialize());
            this.hookStore.Add(ZombieSkinOverrideResolver.Initialize());
        }

        var assetRegistry = AddressableAssetRegistry.Create(ModInfo.Name);

        using var customSkinLoader = new CustomSkinLoader(Melon<Core>.Logger, context.DataService, assetRegistry);

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

        this.quickSwap = new PlantSkinQuickSwap(plantPickerController.Pickers);
    }

    protected override void OnDeinitialize()
    {
        this.hookStore.DetachAll();
        this.quickSwap?.Dispose();
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
