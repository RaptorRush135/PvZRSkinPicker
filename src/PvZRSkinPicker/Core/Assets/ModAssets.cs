namespace PvZRSkinPicker.Assets;

using System.Diagnostics.Contracts;

using UnityEngine;

internal static class ModAssets
{
    public static readonly EmbeddedResourceAsset SkinSwap = new("SkinSwap.png");

    private const FilterMode DefaultFilterMode = FilterMode.Bilinear;

    [Pure]
    public static Sprite LoadSprite(IModAsset asset, FilterMode filterMode = DefaultFilterMode)
    {
        ArgumentNullException.ThrowIfNull(asset);

        var texture = LoadTexture(asset, filterMode);

        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f);
    }

    [Pure]
    public static Texture2D LoadTexture(IModAsset asset, FilterMode filterMode = DefaultFilterMode)
    {
        ArgumentNullException.ThrowIfNull(asset);

        var texture = new Texture2D(1, 1, TextureFormat.RGBA32, mipChain: true)
        {
            filterMode = filterMode,
        };

        var imageBytes = asset.LoadBytes();
        if (!texture.LoadImage(imageBytes))
        {
            throw new InvalidDataException(
                $"Asset '{asset}' could not be decoded.");
        }

        return texture;
    }
}
