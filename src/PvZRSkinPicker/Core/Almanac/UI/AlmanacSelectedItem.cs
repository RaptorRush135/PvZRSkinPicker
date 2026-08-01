namespace PvZRSkinPicker.Almanac.UI;

using Il2CppReloaded.Data;

using Il2CppTekly.DataModels.Binders;

using SolarApi.Unity.Extensions;

using UnityEngine;
using UnityEngine.UI;

internal sealed class AlmanacSelectedItem
{
    public const string PanelName = "SelectedItem";

    private AlmanacSelectedItem(GameObject selectedItem)
    {
        this.Transform = selectedItem.transform.Cast<RectTransform>();

        this.PortraitTransform = this.Transform
            .FindOrThrow("SelectedItemRenderPortrait")
            .Cast<RectTransform>();

        this.NameBinder = this.Transform
            .FindOrThrow("SelectedItemName")
            .GetComponent<StringBinder>();
    }

    public RectTransform Transform { get; }

    public RectTransform PortraitTransform { get; }

    public StringBinder NameBinder { get; }

    public static AlmanacSelectedItem Setup(AlmanacEntryType type)
    {
        var selectedItem = AlmanacUI.
            GetAlmanacContainer(type)
            .FindOrThrow($"Canvas/Layout/Center/Panel/{PanelName}")
            .gameObject;

        var item = new AlmanacSelectedItem(selectedItem);

        item.Transform
            .FindOrThrow("SelectedItemPanel")
            .GetComponent<Image>()
            .raycastTarget = false;

        return item;
    }

    public static AlmanacSelectedItem Wrap(GameObject selectedItem)
        => new(selectedItem);
}
