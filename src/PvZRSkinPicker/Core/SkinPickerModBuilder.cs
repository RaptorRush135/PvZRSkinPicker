namespace PvZRSkinPicker;

using Microsoft.Extensions.DependencyInjection;

using PvZRSkinPicker.Environment;
using PvZRSkinPicker.Metadata;
using PvZRSkinPicker.Skins;
using PvZRSkinPicker.Skins.Prefabs;

using SolarApi;
using SolarApi.Collections;

internal sealed class SkinPickerModBuilder
    : SolarModBuilder<SkinPickerMod>
{
    protected override void ConfigureModServices(IServiceCollection services)
    {
        services.AddSingleton<SkinOverrideResolverManager>();
        services.AddSingleton<SkinPickerModEnvironment>(_ => new(ModInfo.Name));
        services.AddSingleton<ModContext>();
        services.AddSingleton<SkinLocator>();
        services.AddSingleton<DisposeGroup>();
    }
}
