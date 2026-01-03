namespace PvZRSkinPicker.Skins.Prefabs.Plants.Patches.PreOrder;

#pragma warning disable S3265 // Non-flags enums should not be used in bitwise operations
#pragma warning disable RCS1130 // Bitwise operation on enum without Flags attribute

using HarmonyLib;

using Il2CppReloaded;
using Il2CppReloaded.Data;
using Il2CppReloaded.Gameplay;

[HarmonyPatch(typeof(GameplayPooler), nameof(GameplayPooler.GetEffectController))]
internal static class GetEffectControllerPatch
{
    private static readonly ParticleEffect PreorderFlag
        = PreOrderConditionPatchHelper.GetUniquePreOrderKey<ParticleEffect>();

    [HarmonyPrefix]
    private static void Prefix(ParticleEffectData effectData)
    {
        if (effectData.ParticleEffect == ParticleEffect.PeashooterPeaSplat
            && PreOrderConditionPatchHelper.IsContentActive)
        {
            effectData.m_particleEffect |= PreorderFlag;
        }
    }

    [HarmonyPostfix]
    private static void Postfix(ParticleEffectData effectData)
    {
        effectData.m_particleEffect &= ~PreorderFlag;
    }
}
