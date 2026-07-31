namespace PvZRSkinPicker;

using Il2CppReloaded.TreeStateActivities;

using PvZRSkinPicker.Almanac.SeedPackets.Renderer;
using PvZRSkinPicker.Environment;
using PvZRSkinPicker.Skins.Custom;
using PvZRSkinPicker.Skins.Picker;
using PvZRSkinPicker.Skins.Picker.SeedChooser;
using PvZRSkinPicker.Skins.Picker.Selection;
using PvZRSkinPicker.Skins.Prefabs;

using SolarApi;
using SolarApi.Collections;
using SolarApi.IO.Extensions;

internal sealed class SkinPickerMod(
    SkinOverrideResolverManager skinOverrideResolverManager,
    SkinPickerModEnvironment environment,
    SkinPackLoader skinPackLoader,
    SkinPickerControllerInitializer controllerInitializer,
    GameplayActivity gameplayActivity,
    PacketRenderer packetRenderer,
    DisposeGroup disposeGroup)
    : SolarMod
{
    protected override void OnInitialize()
    {
        skinOverrideResolverManager.Initialize();

        var skinSelectionPersistence = new SkinSelectionPersistence(
            environment.ModUserDataDirectory.GetFile("selections.json"));

        SkinSelections skinSelections = skinSelectionPersistence.TryReadSelections();

        CustomSkinSet customSkins = skinPackLoader.GetSkins();

        SkinPickerControllerPair controllerPair = controllerInitializer.Create(skinSelections, customSkins);

        skinSelectionPersistence.BindControllers(controllerPair);

        disposeGroup.Collect(SeedChooserSkinPicker.Initialize());
        disposeGroup.Collect(PlantSkinQuickSwap.Initialize(gameplayActivity, controllerPair.Plant.Pickers));

        packetRenderer.Dispose();
    }
}
