namespace PvZRSkinPicker.Skins.Picker.Selection;

using Il2CppReloaded.Data;

using Microsoft.Extensions.Logging;

using PvZRSkinPicker.Api;
using PvZRSkinPicker.Skins.Picker;

using SolarApi;

internal sealed class SkinSelectionPersistence(
    FileInfo file)
{
    private static readonly ILogger<SkinSelectionPersistence> Logger
        = Solar<SkinPickerMod>.GetLogger<SkinSelectionPersistence>();

    public SkinSelections Current { get; private set; } = SkinSelections.Empty;

    public SkinSelections TryReadSelections()
    {
        try
        {
            if (!file.Exists)
            {
                return SkinSelections.Empty;
            }

            using var fileStream = file.OpenRead();
            var config = SkinSelectionConfig.Load(fileStream);
            this.Current = SkinSelections.Parse(config, Logger);
            return this.Current;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load skin selections at '{FileName}'", file.FullName);
            return SkinSelections.Empty;
        }
    }

    public void TryWriteSelections(SkinSelections selections)
    {
        this.Current = selections;

        try
        {
            using var fileStream = file.Create();
            selections.ToConfig().Write(fileStream);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to save skin selections at '{FileName}'", file.FullName);
        }
    }

    public void BindControllers(SkinPickerControllerPair controllerPair)
    {
        AlmanacApi.OnAlmanacClosed.Subscribe(closeType =>
        {
            SkinSelections newSelections = GetNewSelections();

            this.TryWriteSelections(newSelections);

            SkinSelections GetNewSelections()
            {
                return closeType switch
                {
                    AlmanacEntryType.Plant => this.Current with
                    {
                        Plants = controllerPair.Plant.GetSelections(),
                    },
                    AlmanacEntryType.Zombie => this.Current with
                    {
                        Zombies = controllerPair.Zombie.GetSelections(),
                    },
                };
            }
        });
    }
}
