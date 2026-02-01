namespace PvZRSkinPicker.Tests;

using PvZRSkinPicker.Skins.Custom.Manifest;

public class Version1Tests
{
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

    [Fact]
    public void PlantsFieldIsEmptyWhenMissing()
    {
        var manifest = SkinPackManifest.Parse("""
                {
                    "format_version": 1,
                    "header":
                    {
                        "name": "Test skin pack",
                        "id": "0016f97b-25af-46d5-8caa-0cf0c190196a",
                        "version": 1
                    },
                    "skins": { }
                }
                """);

        Assert.Empty(manifest.Skins.Plants);
    }
}
