using UnityEngine;

public class GridCell
{
    public Vector2Int Position { get; private set; }

    // Things occupying this cell.
    public GameObject Block { get; set; }
    public GameObject WorldObject { get; set; }
    public GameObject Item { get; set; }

    public GridCell(Vector2Int position)
    {
        Position = position;
    }
}