namespace PvZRSkinPicker;

using PvZRSkinPicker.Almanac.SeedPackets.Renderer;
using PvZRSkinPicker.Environment;
using PvZRSkinPicker.Skins.Custom;
using PvZRSkinPicker.Skins.Picker;
using PvZRSkinPicker.Skins.Picker.Selection;
using PvZRSkinPicker.Skins.Prefabs;

using SolarApi;
using SolarApi.Collections;
using SolarApi.IO.Extensions;

internal sealed class SkinPickerMod(
    SkinOverrideResolverManager skinOverrideResolverManager,
    SkinPickerModEnvironment environment,
    CustomSkinLoader customSkinLoader,
    SkinPickerControllerInitializer controllerInitializer,
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

        CustomSkinSet customSkins = customSkinLoader.GetSkins();

        SkinPickerControllerPair controllerPair = controllerInitializer.Create(skinSelections, customSkins);

        skinSelectionPersistence.BindControllers(controllerPair);

        disposeGroup.Collect(PlantSkinQuickSwap.Initialize(controllerPair.Plant.Pickers));

        packetRenderer.Dispose();
    }
}
