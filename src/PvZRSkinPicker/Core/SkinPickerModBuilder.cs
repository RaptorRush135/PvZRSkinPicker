namespace PvZRSkinPicker;

using Microsoft.Extensions.DependencyInjection;

using PvZRSkinPicker.Skins;
using PvZRSkinPicker.Skins.Prefabs;

using SolarApi;
using SolarApi.Collections;

internal sealed class SkinPickerModBuilder
    : SolarModBuilder<SkinPickerMod>
{
    protected override void ConfigureModServices(IServiceCollection services)
    {
        services.AddSingleton<ModContext>();
        services.AddSingleton<DisposeGroup>();
        services.AddSingleton<SkinOverrideResolverManager>();
        services.AddSingleton<SkinLocator>();
    }
}
