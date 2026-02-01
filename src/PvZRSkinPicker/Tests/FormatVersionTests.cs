namespace PvZRSkinPicker.Tests;

using PvZRSkinPicker.Skins.Custom.Manifest;

public class FormatVersionTests
{
    [Fact]
    public void FormatVersionMustBeDefined()
    {
        var ex = Assert.Throws<InvalidDataException>(() => SkinPackManifest.Parse("{}"));
        Assert.Contains(SkinPackManifest.FormatVersionKey, ex.Message);
    }

    [Fact]
    public void FormatVersionMustBeAInteger()
    {
        var ex = Assert.Throws<InvalidDataException>(() => SkinPackManifest.Parse("""
                {
                    "format_version": "invalid"
                }
                """));
        Assert.Contains(SkinPackManifest.FormatVersionKey, ex.Message);
    }
}
