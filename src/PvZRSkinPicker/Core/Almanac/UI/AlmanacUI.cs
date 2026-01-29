namespace PvZRSkinPicker.Almanac.UI;

using Il2CppReloaded.Data;

using PvZRSkinPicker.Unity.Extensions;

using UnityEngine;
using UnityEngine.UI;

internal static class AlmanacUI
{
    private static Transform GlobalPanels => field ??= GameObject.FindOrThrow("GlobalPanels(Clone)").transform;

    public static ModButton CreatePortraitOverlayButton(
        string name,
        Sprite sprite,
        AlmanacEntryType type)
    {
        var selectedItem = GlobalPanels
            .Find($"P_Almanac_{type}s/Canvas/Layout/Center/Panel/SelectedItem")
            .Cast<RectTransform>();

        selectedItem
            .Find("SelectedItemPanel")
            .GetComponent<Image>()
            .raycastTarget = false;

        var portrait = selectedItem
            .Find("SelectedItemRenderPortrait")
            .Cast<RectTransform>();

        int verticalPadding = type == AlmanacEntryType.Plant ? 100 : 75;

        return CreateOverlayButton(name, portrait, sprite, 150, new Vector2(50, verticalPadding));
    }

    private static ModButton CreateOverlayButton(
        string name,
        RectTransform container,
        Sprite sprite,
        int size,
        Vector2 padding)
    {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(sprite);

        var buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(container, worldPositionStays: false);

        var transform = buttonObject.AddComponent<RectTransform>();
        transform.anchorMin = Vector2.one;
        transform.anchorMax = Vector2.one;
        transform.pivot = Vector2.one;
        transform.sizeDelta = Vector2.one * size;
        transform.anchoredPosition = -padding;

        buttonObject.AddComponent<CanvasRenderer>();

        var image = buttonObject.AddComponent<Image>();
        image.sprite = sprite;

        var button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.navigation = new Navigation
        {
            mode = Navigation.Mode.None,
        };

        var colors = button.colors;
        colors.highlightedColor = new Color32(230, 230, 230, 255);
        button.colors = colors;

        return new(button);
    }
}
