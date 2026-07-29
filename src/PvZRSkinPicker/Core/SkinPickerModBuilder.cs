namespace PvZRSkinPicker;

using Il2CppReloaded.Gameplay;

using Microsoft.Extensions.DependencyInjection;

using PvZRSkinPicker.Almanac.SeedPackets.Renderer;
using PvZRSkinPicker.Environment;
using PvZRSkinPicker.Metadata;
using PvZRSkinPicker.Skins;
using PvZRSkinPicker.Skins.Custom;
using PvZRSkinPicker.Skins.Picker;
using PvZRSkinPicker.Skins.Prefabs;
using PvZRSkinPicker.Skins.Prefabs.Plants;
using PvZRSkinPicker.Skins.Prefabs.Zombies;

using SolarApi;
using SolarApi.Collections;
using SolarApi.Unity.Resources;

using UnityEngine;

internal sealed class SkinPickerModBuilder
    : SolarModBuilder<SkinPickerMod>
{
    public override string ShortName => "SkinPicker";

    protected override void ConfigureModServices(IServiceCollection services)
    {
        services.AddSingleton<SkinOverrideResolverManager>();

        services.AddSingleton(_ => new SkinPickerModEnvironment(ModInfo.Name));
        services.AddSingleton(_ => AddressableAssetRegistry.Create(ModInfo.Name));
        services.AddSingleton(_ => PacketRenderer.Create(new Vector2(0, -200)));

        AddSkinOverrideResolver<SeedType, PlantSkinOverrideResolver>();
        AddSkinOverrideResolver<ZombieType, ZombieSkinOverrideResolver>();

        services.AddSingleton<CustomSkinLoader>();
        services.AddSingleton<ISkinTypeHandler<SeedType>, PlantSkinHandler>();
        services.AddSingleton<SkinLoaderFactory>();

        services.AddSingleton<SkinPickerControllerInitializer>();
        services.AddSingleton<SkinLocator>();

        services.AddSingleton<DisposeGroup>();

        void AddSkinOverrideResolver<T, TResolver>()
            where T : struct, Enum
            where TResolver : SkinOverrideResolver<T>
        {
            services.AddSingleton<TResolver>();
            services.AddTransient<SpawnContextContainer<T>>();
        }
    }
}
