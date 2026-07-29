namespace PvZRSkinPicker.Almanac.SeedPackets;

using Il2CppReloaded.DataModels;
using Il2CppReloaded.Gameplay;

using Il2CppSource.DataModels;

using Microsoft.Extensions.DependencyInjection;

using PvZRSkinPicker.Api;

using SolarApi;

internal sealed class PlantPacketThumbnailLookup : PacketThumbnailLookup<SeedType>
{
    private AlmanacEntriesModel? almanacEntries;

    private PlantPacketThumbnailLookup()
    {
    }

    public static PlantPacketThumbnailLookup Instance { get; } = new();

    protected override AlmanacEntriesModel? GetAlmanacEntries()
        => this.almanacEntries ??= Solar<SkinPickerMod>.Provider.GetRequiredService<AlmanacModel>().m_plantsModel;

    protected override SeedChooserDataModel? GetChooserEntries()
        => GameplayDataProviderApi.CurrentModel?.m_seedChooserDataModel;

    protected override SeedBankDataModel? GetBankEntries(bool player2)
        => !player2
            ? GameplayDataProviderApi.CurrentModel?.m_seedBankDataModel
            : GameplayDataProviderApi.CurrentModel?.m_player2DataModel.m_seedBankDataModel;

    protected override SeedType GetEntryDataType(AlmanacEntryModel entry)
        => entry.m_entryData.SeedType;

    protected override SeedType GetEntryDataType(SeedChooserEntryModel entry)
        => entry.m_defData.SeedType;

    protected override SeedType GetEntryDataType(SeedBankEntryModel entry)
    {
        var type = entry.Packet.PacketType;
        return type == SeedType.Imitater
            ? entry.Packet.mImitaterType
            : type;
    }
}
