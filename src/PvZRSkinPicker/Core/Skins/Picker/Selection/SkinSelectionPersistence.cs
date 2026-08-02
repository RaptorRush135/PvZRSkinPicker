namespace PvZRSkinPicker.Skins.Picker.Selection;

using Il2CppReloaded.Data;

using MelonLoader;

using Microsoft.Extensions.Logging;

using PvZRSkinPicker.Api;
using PvZRSkinPicker.Skins.Picker;

using SolarApi;
using SolarApi.Utilities;

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

    public void BindControllersAndScene(
        SkinPickerControllerPair controllerPair,
        string targetSceneName,
        out IDisposable sceneUnloadSubscription)
    {
        AlmanacApi.OnAlmanacClosed.Subscribe(closeType => this.SaveSelections(controllerPair, closeType));

        MelonEvents.OnSceneWasUnloaded.Subscribe(SceneWasUnloadedHandler);

        sceneUnloadSubscription = new DisposableAction(
            () => MelonEvents.OnSceneWasUnloaded.Unsubscribe(SceneWasUnloadedHandler));

        void SceneWasUnloadedHandler(int buildIndex, string sceneName)
        {
            _ = buildIndex;
            if (sceneName == targetSceneName)
            {
                this.SaveSelections(controllerPair, AlmanacEntryType.Plant);
            }
        }
    }

    private void SaveSelections(SkinPickerControllerPair controllerPair, AlmanacEntryType type)
    {
        SkinSelections newSelections = GetNewSelections();

        this.TryWriteSelections(newSelections);

        SkinSelections GetNewSelections()
        {
            return type switch
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
    }
}
