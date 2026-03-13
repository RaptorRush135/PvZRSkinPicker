namespace PvZRSkinPicker.Skins;

using System.Diagnostics.CodeAnalysis;

internal sealed record SkinId(SkinType Type, string Id)
{
    public static SkinId Create(SkinType type)
        => new(type, type.ToString());

    public static SkinId CreateCustom(Guid id)
        => new(SkinType.Custom, id.ToString());

    public static bool TryParse(string id, [MaybeNullWhen(false)] out SkinId skinId)
    {
        const int GuidStringLength = 36;
        if (id.Length == GuidStringLength)
        {
            if (!Guid.TryParse(id, out var guid))
            {
                return Fail(out skinId);
            }

            skinId = CreateCustom(guid);
            return true;
        }

        if (!Enum.TryParse<SkinType>(id, ignoreCase: true, out var skinType)
            || skinType == SkinType.Custom)
        {
            return Fail(out skinId);
        }

        skinId = Create(skinType);
        return true;

        static bool Fail(out SkinId? skinId)
        {
            skinId = null;
            return false;
        }
    }
}
