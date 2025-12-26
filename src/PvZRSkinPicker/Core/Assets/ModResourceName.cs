namespace PvZRSkinPicker.Assets;

internal class ModResourceName
{
    private readonly string name;

    public ModResourceName(string fileName)
    {
        ArgumentNullException.ThrowIfNull(fileName);
        this.name = ModAssets.GetResourceName(fileName);
    }

    public static implicit operator ModResourceName(string fileName) => new(fileName);

    public static implicit operator string(ModResourceName resource) => resource.name;

    public override string ToString() => this;

    public string GetNameWithoutExtension() => Path.GetFileNameWithoutExtension(this);
}
