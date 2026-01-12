namespace PvZRSkinPicker.Skins.Prefabs;

using Il2CppReloaded.Gameplay;

internal readonly record struct SpawnContext<T>(T Type, Board? Board, int Row)
    where T : struct, Enum
{
    public bool IsAtRowType(PlantRowType rowType)
    {
        return this.Board?.mPlantRow[this.Row] == rowType;
    }
}
