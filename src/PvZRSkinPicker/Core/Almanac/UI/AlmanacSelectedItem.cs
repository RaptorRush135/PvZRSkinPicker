namespace PvZRSkinPicker.Almanac.UI;

using Il2CppReloaded.Data;

using Il2CppTekly.DataModels.Binders;

using PvZRSkinPicker.Unity.Extensions;

using UnityEngine;
using UnityEngine.UI;

internal sealed class AlmanacSelectedItem
{
    private AlmanacSelectedItem(AlmanacEntryType type)
    {
        var selectedItem = GlobalPanels
            .Find($"P_Almanac_{type}s/Canvas/Layout/Center/Panel/SelectedItem")
            .Cast<RectTransform>();

        this.Transform = selectedItem;

        this.PortraitTransform = selectedItem
            .Find("SelectedItemRenderPortrait")
            .Cast<RectTransform>();

        this.NameBinder = selectedItem
            .Find("SelectedItemName")
            .GetComponent<StringBinder>();
    }

    public RectTransform Transform { get; }

    public RectTransform PortraitTransform { get; }

    public StringBinder NameBinder { get; }

    private static Transform GlobalPanels
        => field ??= GameObject.FindOrThrow("GlobalPanels(Clone)").transform;

    public static AlmanacSelectedItem Setup(AlmanacEntryType type)
    {
        var item = new AlmanacSelectedItem(type);

        item.Transform
            .Find("SelectedItemPanel")
            .GetComponent<Image>()
            .raycastTarget = false;

        return item;
    }
}
