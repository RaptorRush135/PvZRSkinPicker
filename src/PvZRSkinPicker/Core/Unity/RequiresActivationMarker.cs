namespace PvZRSkinPicker.Unity;

using MelonLoader;

using UnityEngine;

[RegisterTypeInIl2Cpp]
internal sealed class RequiresActivationMarker(IntPtr ptr)
    : MonoBehaviour(ptr)
{
    public static void ActivateIfRequired(GameObject gameObject)
    {
        if (gameObject.TryGetComponent<RequiresActivationMarker>(out var marker))
        {
            marker.Activate();
        }
    }

    public void Activate()
    {
        this.gameObject.SetActive(true);
        Destroy(this);
    }
}
