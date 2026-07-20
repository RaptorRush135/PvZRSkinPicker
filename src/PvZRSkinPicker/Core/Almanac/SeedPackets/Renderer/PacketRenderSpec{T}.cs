namespace PvZRSkinPicker.Almanac.SeedPackets.Renderer;

using PvZRSkinPicker.Skins.Custom.Manifest;

internal readonly record struct PacketRenderSpec<T>(
    T Type,
    SkinTransform Transform)
    where T : struct, Enum;
