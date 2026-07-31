namespace PvZRSkinPicker.Skins.Picker.SeedChooser.Patches;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Gameplay;

using Il2CppTekly.DataModels.Binders;

using PvZRSkinPicker.Almanac.UI;

[Harmony]
internal static class DefaultSeedWhenNoneSelectedPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(BinderKeyProxy), nameof(BinderKeyProxy.BindKey))]
    private static void BindKeyPrefix(BinderKeyProxy __instance, ref string value)
    {
        if (!string.IsNullOrEmpty(value)
            || __instance.m_key.Path != SeedChooserSkinPicker.SelectionModelKey)
        {
            return;
        }

        string objectName = __instance.gameObject.name;
        if (objectName.StartsWith(AlmanacSelectedItem.PanelName)
            || objectName.StartsWith(SeedChooserSkinPicker.PortraitRenderName))
        {
            value = ((int)SeedType.Peashooter).ToString();
        }
    }
}
