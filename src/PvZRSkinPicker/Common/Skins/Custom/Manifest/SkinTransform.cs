namespace PvZRSkinPicker.Skins.Custom.Manifest;

internal readonly record struct SkinTransform(
    float X,
    float Y,
    float Scale = 1)
{
    public SkinTransform Apply(SkinTransformOverride? @override)
    {
        return @override is { } value
            ? this.Apply(value)
            : this;
    }

    public SkinTransform Apply(SkinTransformOverride @override)
    {
        return new SkinTransform(
            @override.X ?? this.X,
            @override.Y ?? this.Y,
            @override.Scale ?? this.Scale);
    }
}
