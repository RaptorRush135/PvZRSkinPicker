namespace PvZRSkinPicker;

using Microsoft.Extensions.DependencyInjection;

using PvZRSkinPicker.Skins;

using SolarApi;

internal sealed class SkinPickerModBuilder
    : SolarModBuilder<SkinPickerMod>
{
    protected override void ConfigureModServices(IServiceCollection services)
    {
        services.AddSingleton<ModContext>();
        services.AddSingleton<SkinLocator>();
    }
}
