namespace PvZRSkinPicker.Assets;

using System.Diagnostics.Contracts;
using System.Reflection;

using PvZRSkinPicker.Extensions;

using UnityEngine;

internal static class ModAssets
{
    [Pure]
    public static string GetResourceName(string fileName)
    {
        return $"{nameof(PvZRSkinPicker)}.{nameof(Assets)}.{fileName}";
    }

    [Pure]
    public static Sprite LoadSprite(ModResourceName resourceName)
    {
        var texture = LoadTexture(resourceName);

        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f);
    }

    [Pure]
    public static Texture2D LoadTexture(ModResourceName resourceName)
    {
        var texture = new Texture2D(1, 1, TextureFormat.RGBA32, mipChain: true);
        var imageBytes = LoadBytes(resourceName);
        if (!texture.LoadImage(imageBytes))
        {
            throw new InvalidDataException(
                $"Resource '{resourceName}' could not be decoded.");
        }

        return texture;
    }

    [Pure]
    public static byte[] LoadBytes(ModResourceName resourceName)
    {
        using var stream = LoadStream(resourceName);
        return stream.ToArray();
    }

    [Pure]
    public static Stream LoadStream(ModResourceName resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        return assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Resource not found: '{resourceName}'.");
    }
}
