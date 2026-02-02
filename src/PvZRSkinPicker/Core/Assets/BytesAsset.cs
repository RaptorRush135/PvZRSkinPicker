namespace PvZRSkinPicker.Assets;

internal sealed class BytesAsset(byte[] bytes) : IModAsset
{
    public byte[] LoadBytes() => bytes;
}
