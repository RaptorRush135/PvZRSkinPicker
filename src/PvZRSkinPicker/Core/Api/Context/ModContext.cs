namespace PvZRSkinPicker.Api.Context;

using Il2CppReloaded.DataModels;
using Il2CppReloaded.Services;

internal sealed record ModContext(
    IDataService DataService,
    IPlatformService PlatformService,
    AlmanacModel Almanac);
