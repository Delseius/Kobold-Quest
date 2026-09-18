using UnityEngine;

public class GridCell
{
    public Vector2Int Position { get; private set; }

    // Things occupying this cell.
    public GameObject Block { get; set; } //things you might be able to pick up/walls 
    public GameObject WorldObject { get; set; } // background/ immovable objects cave entrance, 

    public GameObject Tile { get; set; } // Optional GameObject-based tile, registered by GridObject.
    public GameObject Item { get; set; }

    // Data for terrain drawn by a Unity Tilemap (not a separate GameObject).
    public GridTile TerrainTile { get; internal set; }

    public GridCell(Vector2Int position)
    {
        Position = position;
    }
}
