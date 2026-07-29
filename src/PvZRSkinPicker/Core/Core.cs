[assembly: MelonLoader.HarmonyDontPatchAll]

namespace PvZRSkinPicker;

using MelonLoader;

using SolarApi;

public sealed class Core : MelonMod
{
    public override void OnInitializeMelon()
    {
        Solar<SkinPickerMod>.RegisterMod<SkinPickerModBuilder>(this);
    }
}
