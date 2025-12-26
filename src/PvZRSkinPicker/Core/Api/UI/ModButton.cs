namespace PvZRSkinPicker.Api.UI;

using UnityEngine.UI;

internal sealed class ModButton(Button button)
{
    public void AddOnClick(Action action)
    {
        button.onClick.AddListener(action);
    }

    public void SetActive(bool active)
    {
        button.gameObject.SetActive(active);
    }
}
