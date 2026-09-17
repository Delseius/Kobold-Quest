using System.Collections.Generic;
using UnityEngine;

public class GridSpace : MonoBehaviour
{
    public static GridSpace Instance { get; private set; }

    [Header("Grid Settings")]
    [SerializeField] private float cellSize = 0.5f;

    private Dictionary<Vector2Int, GridCell> cells =
        new Dictionary<Vector2Int, GridCell>();

    private void Awake()
    {
        // Make sure there is only one GridSpace in the scene.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Converts a Unity world position into a grid coordinate.
    /// </summary>
    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / cellSize);
        int y = Mathf.FloorToInt(worldPosition.y / cellSize);

        return new Vector2Int(x, y);
    }

    /// <summary>
    /// Converts a grid coordinate into the centre of that grid cell.
    /// </summary>
    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        float x = (gridPosition.x + 0.5f) * cellSize;
        float y = (gridPosition.y + 0.5f) * cellSize;

        return new Vector3(x, y, 0f);
    }

    /// <summary>
    /// Gets the GridCell at the specified grid position.
    /// Creates the cell if it doesn't already exist.
    /// </summary>
    public GridCell GetCell(Vector2Int gridPosition)
    {
        if (!cells.ContainsKey(gridPosition))
        {
            cells[gridPosition] = new GridCell(gridPosition);
        }

        return cells[gridPosition];
    }

    /// <summary>
    /// Gets the grid cell containing a particular world position.
    /// </summary>
    public GridCell GetCellFromWorldPosition(Vector3 worldPosition)
    {
        Vector2Int gridPosition = WorldToGrid(worldPosition);

        return GetCell(gridPosition);
    }

    /// <summary>
    /// Returns the size of one grid cell in Unity units.
    /// </summary>
    public float GetCellSize()
    {
        return cellSize;
    }
    public void DebugCell(Vector2Int position)
    {
        GridCell cell = GetCell(position);

        Debug.Log(
            "Cell " + position +
            " | Block: " + cell.Block +
            " | World Object: " + cell.WorldObject +
            " | Item: " + cell.Item
        );
    }
}