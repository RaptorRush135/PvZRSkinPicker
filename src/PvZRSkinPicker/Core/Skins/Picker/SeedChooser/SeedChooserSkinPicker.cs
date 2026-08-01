namespace PvZRSkinPicker.Skins.Picker.SeedChooser;

using Il2CppReloaded;
using Il2CppReloaded.Gameplay;

using Il2CppTekly.DataModels.Binders;
using Il2CppTekly.DataModels.Models;

using MelonLoader;

using Microsoft.Extensions.Logging;

using PvZRSkinPicker.Almanac;
using PvZRSkinPicker.Almanac.UI;
using PvZRSkinPicker.Api;

using SolarApi;
using SolarApi.Unity.Extensions;

using UnityEngine;

internal sealed class SeedChooserSkinPicker : IDisposable
{
    public const string SelectionModelKey = "gameplay.seedChooser.selected";

    public const string PortraitRenderName = "P_AlmanacPortraitRender";

    private readonly ILogger<SeedChooserSkinPicker> logger
        = Solar<SkinPickerMod>.GetLogger<SeedChooserSkinPicker>();

    private readonly IReadOnlyDictionary<SeedType, SkinPicker<SeedType>> pickers;

    private bool disposed;

    private SeedChooserSkinPicker(
        IReadOnlyDictionary<SeedType, SkinPicker<SeedType>> pickers)
    {
        this.pickers = pickers;
    }

    public static SeedChooserSkinPicker Initialize(
        IReadOnlyDictionary<SeedType, SkinPicker<SeedType>> pickers)
    {
        var instance = new SeedChooserSkinPicker(pickers);
        MelonEvents.OnSceneWasLoaded.Subscribe(instance.OnSceneWasLoaded);
        return instance;
    }

    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.disposed = true;
        MelonEvents.OnSceneWasLoaded.Unsubscribe(this.OnSceneWasLoaded);
    }

    private static RectTransform CloneAlmanacSelectedPlantPanel(BinderContainer container)
    {
        var portraitClone = ClonePortraitRender();

        var binderKeyProxy = portraitClone.GetComponent<BinderKeyProxy>();

        var selectionModelRef = ModelRef.Create(SelectionModelKey);

        binderKeyProxy.m_key = selectionModelRef;

        var almanacSelectedItemPanel = AlmanacUI.PlantSelectedItem.Transform.gameObject;

        var panelClone = Object.Instantiate(
            almanacSelectedItemPanel,
            container.transform,
            worldPositionStays: false);

        var panelBinder = panelClone.GetComponent<BinderContainer>();
        panelBinder.m_key = selectionModelRef;
        panelBinder.m_bindOnEnable = true;

        var keyProxy = panelClone.AddComponent<BinderKeyProxy>();
        keyProxy.m_key = selectionModelRef;
        keyProxy.m_target = panelBinder;
        keyProxy.m_keyFormat = binderKeyProxy.m_keyFormat;

        container.m_binders.Add(keyProxy);

        return panelClone.transform.Cast<RectTransform>();

        static GameObject ClonePortraitRender()
        {
            var portraitTransform = AlmanacUI.PlantAlmanacContainer.FindOrThrow(PortraitRenderName);

            return Object.Instantiate(
                portraitTransform.gameObject,
                portraitTransform.position - new Vector3(1500, 0),
                Quaternion.identity);
        }
    }

    private void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (sceneName == Constants.Transition.GAMEPLAY)
        {
            var gameplayDataModel = GameplayDataProviderApi.CurrentModel;
            if (gameplayDataModel == null)
            {
                this.logger.LogWarning("Gameplay data model not available");
                return;
            }

            this.GameplaySceneSetup(gameplayDataModel.m_seedChooserDataModel.m_selectedModel);
        }
    }

    private void GameplaySceneSetup(StringValueModel seedChooserSelectedModel)
    {
        // TODO: Bind PortraitRender visbility to chooser

        var seedChooserTransform = GameObject.FindOrThrow("Panels")
            .transform.FindOrThrow("SeedChooserPanels/P_SeedChooser/Canvas/Layout/Center/Panel/SeedChooser")
            .GetComponent<BinderContainer>();

        var selectedPlantPanelTransform = CloneAlmanacSelectedPlantPanel(seedChooserTransform);

        var selectedItem = AlmanacSelectedItem.Wrap(selectedPlantPanelTransform.gameObject);

        var pickerController = CreateController();
        pickerController.RefreshName(overrideUntilNextNameSet: true, mustRunThisFrame: false);

        var button = SkinSwapUI.GetButton(selectedItem);

        pickerController.Bind(button);

        SetupPanel(selectedPlantPanelTransform);

        SkinPickerController<SeedType> CreateController()
        {
            var selection = new AlmanacSelection<SeedType>(
                seedChooserSelectedModel,
                selectedItem.NameBinder,
                allowEmptySelection: true);

            return new(selection, this.pickers);
        }

        static void SetupPanel(RectTransform panel)
        {
            panel.anchorMin = Vector2.one;
            panel.anchorMax = Vector2.one;
            panel.pivot = new Vector2(0, 1);
            panel.anchoredPosition = new Vector2(-5, 0);
            panel.localScale = Vector3.one * 0.8f;
        }
    }
}
