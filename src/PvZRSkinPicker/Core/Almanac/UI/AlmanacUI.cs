namespace PvZRSkinPicker.Almanac.UI;

using Il2CppReloaded.Data;

using SolarApi.Unity.Extensions;

using UnityEngine;
using UnityEngine.UI;

internal static class AlmanacUI
{
    public static Transform PlantAlmanacContainer
        => field ??= FindAlmanacContainer(AlmanacEntryType.Plant);

    public static Transform ZombieAlmanacContainer
        => field ??= FindAlmanacContainer(AlmanacEntryType.Zombie);

    public static AlmanacSelectedItem PlantSelectedItem
        => field ??= AlmanacSelectedItem.Setup(AlmanacEntryType.Plant);

    public static AlmanacSelectedItem ZombieSelectedItem
        => field ??= AlmanacSelectedItem.Setup(AlmanacEntryType.Zombie);

    private static Transform GlobalPanels
        => field ??= GameObject.FindOrThrow("GlobalPanels(Clone)").transform;

    public static ModButton CreatePortraitOverlayButton(
        string name,
        Sprite sprite,
        AlmanacEntryType type)
    {
        var selectedItem = GetSelectedItem(type);

        int verticalPadding = type == AlmanacEntryType.Plant ? 100 : 75;

        return CreateOverlayButton(name, selectedItem.PortraitTransform, sprite, 150, new Vector2(50, verticalPadding));
    }

    public static ModButton GetPortraitOverlayButton(
        string name,
        AlmanacSelectedItem selectedItem)
    {
        var button = selectedItem.PortraitTransform
            .FindOrThrow(name)
            .GetComponent<Button>();

        return new(button);
    }

    public static AlmanacSelectedItem GetSelectedItem(AlmanacEntryType type)
    {
        return type == AlmanacEntryType.Plant
            ? PlantSelectedItem
            : ZombieSelectedItem;
    }

    public static Transform GetAlmanacContainer(AlmanacEntryType type)
    {
        return type == AlmanacEntryType.Plant
            ? PlantAlmanacContainer
            : ZombieAlmanacContainer;
    }

    private static Transform FindAlmanacContainer(AlmanacEntryType type)
        => GlobalPanels.FindOrThrow($"P_Almanac_{type}s");

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
