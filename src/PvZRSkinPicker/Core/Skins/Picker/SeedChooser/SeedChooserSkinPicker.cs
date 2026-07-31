namespace PvZRSkinPicker.Skins.Picker.SeedChooser;

using Il2CppReloaded;

using Il2CppTekly.DataModels.Binders;

using MelonLoader;

using PvZRSkinPicker.Almanac.UI;

using SolarApi.Unity.Extensions;

using UnityEngine;

internal sealed class SeedChooserSkinPicker : IDisposable
{
    public const string SelectionModelKey = "gameplay.seedChooser.selected";

    public const string PortraitRenderName = "P_AlmanacPortraitRender";

    private bool disposed;

    private SeedChooserSkinPicker()
    {
    }

    public static SeedChooserSkinPicker Initialize()
    {
        var instance = new SeedChooserSkinPicker();
        MelonEvents.OnSceneWasLoaded.Subscribe(OnSceneWasLoaded);
        return instance;
    }

    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.disposed = true;
        MelonEvents.OnSceneWasLoaded.Unsubscribe(OnSceneWasLoaded);
    }

    private static void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (sceneName == Constants.Transition.GAMEPLAY)
        {
            GameplaySceneSetup();
        }
    }

    private static void GameplaySceneSetup()
    {
        // TODO: Bind PortraitRender visbility to chooser
        // TODO: Bind skin swap button

        var seedChooserTransform = GameObject.FindOrThrow("Panels")
            .transform.FindOrThrow("SeedChooserPanels/P_SeedChooser/Canvas/Layout/Center/Panel/SeedChooser")
            .GetComponent<BinderContainer>();

        var selectedPlantPanelTransform = CloneAlmanacSelectedPlantPanel(seedChooserTransform);

        SetupPanel(selectedPlantPanelTransform);

        static void SetupPanel(RectTransform panel)
        {
            panel.anchorMin = Vector2.one;
            panel.anchorMax = Vector2.one;
            panel.pivot = new Vector2(0, 1);
            panel.anchoredPosition = new Vector2(-5, 0);
            panel.localScale = Vector3.one * 0.8f;
        }
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
}
