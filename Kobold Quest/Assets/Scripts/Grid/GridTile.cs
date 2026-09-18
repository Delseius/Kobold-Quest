using UnityEngine;

public enum TerrainType
{
    Dirt,
    TilledDirt,
    Grass,
    Water
}

// One record per cell. Tile assets may be shared, but this state is not.
public sealed class GridTile
{
    public Vector2Int Position { get; }
    public TerrainType Type { get; internal set; }
    public TilemapRenderer Owner { get; }

    public GridTile(Vector2Int position, TerrainType type, TilemapRenderer owner)
    {
        Position = position;
        Type = type;
        Owner = owner;
    }

    public bool TryHoe()
    {
        return Owner != null && Owner.TryHoe(Position);
    }
}
