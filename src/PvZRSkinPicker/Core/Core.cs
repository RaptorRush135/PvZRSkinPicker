namespace PvZRSkinPicker;

using Il2CppReloaded.Data;
using Il2CppReloaded.Gameplay;

using MelonLoader;

using PvZRSkinPicker.Api.Context;
using PvZRSkinPicker.Api.UI;
using PvZRSkinPicker.Assets;
using PvZRSkinPicker.Extensions;
using PvZRSkinPicker.Skins;

public sealed class Core : MelonMod
{
    private ModContext? context;

    private Dictionary<SeedType, PlantSkinPicker>? pickers;

    public override void OnInitializeMelon()
    {
        ModContextApi.Ready += this.Ready;
    }

    private void Ready(ModContext context)
    {
        this.pickers =
            context.DataService.PlantDefinitions.AsEnumerable()
            .Select(plant =>
                SkinPicker<SeedType>.TryCreate<PlantSkinPicker>(
                    plant.SeedType,
                    SkinProvider.GetSkins(context.PlatformService, plant)))
            .WhereNotNull()
            .ToDictionary(picker => picker.Type);

        this.context = context;

        const string SkinSwap = "SkinSwap";
        var icon = ModAssets.LoadSprite($"{SkinSwap}.png");
        var plantSkinSwapButton = AlmanacUI.CreatePortraitOverlayButton(SkinSwap, icon, AlmanacEntryType.Plant);
        plantSkinSwapButton.AddOnClick(this.NextPlant);

        this.context.Almanac.m_plantsModel.m_selectedModel.Subscribe(
            (Action<string>)(value =>
            {
                var selectedType = (SeedType)int.Parse(value);
                bool enabled = this.pickers.ContainsKey(selectedType);
                plantSkinSwapButton.SetActive(enabled);
            }));
    }

    private void NextPlant()
    {
        if (this.pickers == null)
        {
            return;
        }

        var selectedModel = this.context.Almanac.m_plantsModel.m_selectedModel;

        var selectedType = (SeedType)int.Parse(selectedModel.Value);

        if (this.pickers.TryGetValue(selectedType, out var picker))
        {
            picker.Next();
            selectedModel.Emit(selectedModel.Value);
        }
    }
}
