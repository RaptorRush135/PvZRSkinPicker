namespace PvZRSkinPicker.Tests;

using Newtonsoft.Json;

using PvZRSkinPicker.Skins.Custom.Manifest;

public class EmptyPackTests
{
    [Fact]
    public void FailOnEmpty()
    {
        Assert.Throws<JsonReaderException>(
            () => SkinPackManifest.Parse(string.Empty));
    }
}
