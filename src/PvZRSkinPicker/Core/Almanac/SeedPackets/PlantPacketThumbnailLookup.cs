namespace PvZRSkinPicker.Almanac.SeedPackets;

using Il2CppReloaded.DataModels;
using Il2CppReloaded.Gameplay;

using Il2CppSource.DataModels;

using PvZRSkinPicker.Api;

internal sealed class PlantPacketThumbnailLookup : PacketThumbnailLookup<SeedType>
{
    private PlantPacketThumbnailLookup()
    {
    }

    public static PlantPacketThumbnailLookup Instance { get; } = new();

    protected override AlmanacEntriesModel? GetAlmanacEntries()
        => AppDataApi.AlmanacPlantEntriesModel;

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
