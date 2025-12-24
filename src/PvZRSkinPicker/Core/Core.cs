namespace PvZRSkinPicker;

using Il2CppReloaded.Gameplay;

using MelonLoader;

using PvZRSkinPicker.Api.Context;
using PvZRSkinPicker.Extensions;
using PvZRSkinPicker.Skins;

using UnityEngine;

public sealed class Core : MelonMod
{
    private ModContext? context;

    private Dictionary<SeedType, PlantSkinPicker>? pickers;

    public override void OnInitializeMelon()
    {
        ModContextApi.Ready += this.Ready;
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.NextPlant();
        }
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
