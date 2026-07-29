namespace PvZRSkinPicker.Almanac.SeedPackets;

using Il2CppInterop.Runtime.InteropTypes;

using Il2CppReloaded.DataModels;

using Il2CppSource.DataModels;

using Il2CppTekly.DataModels.Models;

using Microsoft.Extensions.Logging;

using SolarApi.Collections.Extensions;
using SolarApi.Il2Cpp.Extensions;

internal abstract class PacketThumbnailLookup<T>(
    ILogger<PacketThumbnailLookup<T>> logger)
    where T : struct, Enum
{
    public IEnumerable<AddressableSpriteValueModel> GetThumbnails(T type)
    {
        IEnumerable<AddressableSpriteValueModel?> thumbnails =
        [
            this.GetAlmanacThumbnail(type),
            this.GetChooserThumbnail(type),
            this.GetImitaterChooserThumbnail(type),
            ..this.GetBankThumbnails(type),
            ..this.GetBankThumbnails(type, player2: true),
        ];

        return thumbnails.WhereNotNull();
    }

    public AddressableSpriteValueModel? GetAlmanacThumbnail(T type)
        => this.GetEntries<AlmanacEntriesModel, AlmanacEntryModel>(
        type, this.GetAlmanacEntries, c => c.m_entriesModel, this.GetEntryDataType, true)
        .FirstOrDefault()?.m_thumbnailModel;

    public AddressableSpriteValueModel? GetChooserThumbnail(T type)
        => this.GetEntries<SeedChooserDataModel, SeedChooserEntryModel>(
        type, this.GetChooserEntries, c => c.m_entriesUnlockedModel, this.GetEntryDataType)
        .FirstOrDefault()?.m_thumbnail;

    public AddressableSpriteValueModel? GetImitaterChooserThumbnail(T type)
        => this.GetEntries<SeedChooserDataModel, SeedChooserEntryModel>(
        type, this.GetChooserEntries, c => c.m_imitaterEntriesModel, this.GetEntryDataType)
        .FirstOrDefault()?.m_thumbnail;

    public IEnumerable<AddressableSpriteValueModel> GetBankThumbnails(T type, bool player2 = false)
        => this.GetEntries<SeedBankDataModel, SeedBankEntryModel>(
        type, () => this.GetBankEntries(player2), c => c.m_entriesModel, this.GetEntryDataType)
        .Select(e => e.m_thumbnail);

    protected abstract AlmanacEntriesModel? GetAlmanacEntries();

    protected abstract SeedChooserDataModel? GetChooserEntries();

    protected abstract SeedBankDataModel? GetBankEntries(bool player2);

    protected abstract T GetEntryDataType(AlmanacEntryModel entry);

    protected abstract T GetEntryDataType(SeedChooserEntryModel entry);

    protected abstract T GetEntryDataType(SeedBankEntryModel entry);

    private IEnumerable<TEntry> GetEntries<TContainer, TEntry>(
        T type,
        Func<TContainer?> containerGetter,
        Func<TContainer, ObjectModel> entriesGetter,
        Func<TEntry, T> entryDataTypeGetter,
        bool warnIfContainerMissing = false)
        where TContainer : ObjectModel
        where TEntry : Il2CppObjectBase
    {
        TContainer? container = containerGetter();

        if (container == null)
        {
            if (warnIfContainerMissing)
            {
                logger.LogWarning(
                    "{ContainerName} not available ({TypeName})",
                    typeof(TContainer).Name,
                    typeof(T).Name);
            }

            return [];
        }

        return entriesGetter(container).Models.AsEnumerable()
            .Select(entry => entry.Model.Cast<TEntry>())
            .Where(entry => EqualityComparer<T>.Default.Equals(entryDataTypeGetter(entry), type));
    }
}
