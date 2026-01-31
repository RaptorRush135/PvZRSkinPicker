namespace PvZRSkinPicker.Tests;

using Newtonsoft.Json;

using PvZRSkinPicker.Skins.Custom.Manifest;

public class SkinPackManifestTests
{
    [Fact]
    public void FailOnEmpty()
    {
        Assert.Throws<JsonReaderException>(
            () => SkinPackManifest.Parse(string.Empty));
    }

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

    [Fact]
    public void CanParseVersion1()
    {
        SkinPackManifest.Parse("""
                {
                    "format_version": 1,
                    "header":
                    {
                        "name": "Test skin pack",
                        "id": "0016f97b-25af-46d5-8caa-0cf0c190196a",
                        "version": 1
                    },
                    "skins":
                    {
                        "plants":
                        [
                            {
                                "type": "Sunflower",
                                "name": "My plant",
                                "id": "31767e20-968f-4d53-8021-738116dfd07d",
                                "directory": "MyPlant"
                            }
                        ]
                    }
                }
                """);
    }
}
