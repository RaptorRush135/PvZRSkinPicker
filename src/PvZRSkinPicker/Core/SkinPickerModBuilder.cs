namespace PvZRSkinPicker;

using Microsoft.Extensions.DependencyInjection;

using PvZRSkinPicker.Almanac.SeedPackets.Renderer;
using PvZRSkinPicker.Environment;
using PvZRSkinPicker.Metadata;
using PvZRSkinPicker.Skins;
using PvZRSkinPicker.Skins.Custom;
using PvZRSkinPicker.Skins.Picker;
using PvZRSkinPicker.Skins.Prefabs;

using SolarApi;
using SolarApi.Collections;
using SolarApi.Unity.Resources;

using UnityEngine;

internal sealed class SkinPickerModBuilder
    : SolarModBuilder<SkinPickerMod>
{
    protected override void ConfigureModServices(IServiceCollection services)
    {
        services.AddSingleton<SkinOverrideResolverManager>();
        services.AddSingleton(_ => new SkinPickerModEnvironment(ModInfo.Name));
        services.AddSingleton(_ => AddressableAssetRegistry.Create(ModInfo.Name));
        services.AddSingleton(_ => PacketRenderer.Create(new Vector2(0, -200)));
        services.AddSingleton<CustomSkinLoader>();
        services.AddSingleton<SkinPickerControllerInitializer>();
        services.AddSingleton<SkinLocator>();
        services.AddSingleton<DisposeGroup>();
    }
}
