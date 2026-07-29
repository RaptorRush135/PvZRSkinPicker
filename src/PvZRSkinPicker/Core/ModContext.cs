namespace PvZRSkinPicker;

using Il2CppReloaded.DataModels;
using Il2CppReloaded.Services;

internal sealed record ModContext(
    IDataService DataService,
    AlmanacModel Almanac);
