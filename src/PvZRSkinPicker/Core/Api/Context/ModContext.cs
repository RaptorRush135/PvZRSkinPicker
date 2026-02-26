namespace PvZRSkinPicker.Api.Context;

using Il2CppReloaded.DataModels;
using Il2CppReloaded.Services;

using Il2CppTekly.Localizations;

internal sealed record ModContext(
    IDataService DataService,
    IPlatformService PlatformService,
    ILocalizer Localizer,
    AlmanacModel Almanac);
