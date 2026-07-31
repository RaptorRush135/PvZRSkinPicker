namespace PvZRSkinPicker.Almanac.UI;

using Il2CppReloaded.Data;

using Il2CppTekly.DataModels.Binders;

using UnityEngine;
using UnityEngine.UI;

internal sealed class AlmanacSelectedItem
{
    public const string PanelName = "SelectedItem";

    private AlmanacSelectedItem(AlmanacEntryType type)
    {
        var selectedItem = AlmanacUI.
            GetAlmanacContainer(type)
            .Find($"Canvas/Layout/Center/Panel/{PanelName}")
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
