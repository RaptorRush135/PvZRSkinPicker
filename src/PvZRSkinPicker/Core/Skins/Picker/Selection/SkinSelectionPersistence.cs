namespace PvZRSkinPicker.Skins.Picker.Selection;

using Il2CppReloaded.Data;
using Il2CppReloaded.Gameplay;

using MelonLoader;

using PvZRSkinPicker.Api;

internal sealed class SkinSelectionPersistence(
    FileInfo file)
{
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
            this.Current = SkinSelections.Parse(config);
            return this.Current;
        }
        catch (Exception ex)
        {
            Melon<Core>.Logger.Error($"Failed to load skin selections at '{file.FullName}'", ex);
            return SkinSelections.Empty;
        }
    }

    public void TryWriteSelections(SkinSelections selections)
    {
        this.Current = selections;

        try
        {
            using var fileStream = file.OpenWrite();
            selections.ToConfig().Write(fileStream);
        }
        catch (Exception ex)
        {
            Melon<Core>.Logger.Error($"Failed to save skin selections at '{file.FullName}'", ex);
        }
    }

    public void BindControllers(
        SkinPickerController<SeedType> plantPickerController,
        SkinPickerController<ZombieType> zombiePickerController)
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
                        Plants = plantPickerController.GetSelections(),
                    },
                    AlmanacEntryType.Zombie => this.Current with
                    {
                        Zombies = zombiePickerController.GetSelections(),
                    },
                };
            }
        });
    }
}
