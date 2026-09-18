using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Fully qualify Unity's component because this script has the same name.
[RequireComponent(typeof(Tilemap), typeof(UnityEngine.Tilemaps.TilemapRenderer))]
public class TilemapRenderer : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite tileSprite;
    [SerializeField] private Sprite tilledDirtSprite;


    [Header("Generation")]
    [SerializeField, Min(1)] private int width = 10;
    [SerializeField, Min(1)] private int height = 10;
    [SerializeField] private TerrainType initialTerrain = TerrainType.Dirt;

    private Tilemap tilemap;
    private GridSpace grid;
    private bool ready;
    private readonly Dictionary<TerrainType, Tile> tiles = new Dictionary<TerrainType, Tile>();
    private readonly HashSet<Vector2Int> registeredPositions = new HashSet<Vector2Int>();

    private void Start()
    {
        grid = GridSpace.Instance;
        tilemap = GetComponent<Tilemap>();

        if (grid == null || grid.GetCellSize() <= 0f)
        {
            Debug.LogError("Add a GridSpace with a positive cell size to the scene.", this);
            return;
        }

        // GridSpace assumes an XY grid starting at world (0, 0).
        if (!MatchesGrid(Vector2Int.zero) || !MatchesGrid(Vector2Int.right)
            || !MatchesGrid(Vector2Int.up))
        {
            Debug.LogError("Align the rectangular Tilemap with GridSpace: origin (0,0,0), "
                + "no rotation, unit scale, and matching X/Y cell sizes with no cell gap.", this);
            return;
        }

        AddTile(TerrainType.Dirt, tileSprite);
        AddTile(TerrainType.TilledDirt, tilledDirtSprite);

        if (!tiles.ContainsKey(initialTerrain))
        {
            Debug.LogError("Assign the sprite for the initial terrain type.", this);
            return;
        }

        if (tilledDirtSprite == null)
        {
            Debug.LogWarning("Assign Tilled Dirt Sprite to enable hoeing.", this);
        }

        ready = true;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                DrawTile(x, y);
            }
        }
    }

    private bool MatchesGrid(Vector2Int position)
    {
        Vector3 centre = tilemap.GetCellCenterWorld(new Vector3Int(position.x, position.y, 0));
        return (centre - grid.GridToWorld(position)).sqrMagnitude < 0.000001f;
    }

    private void AddTile(TerrainType type, Sprite sprite)
    {
        if (sprite == null) return;

        Tile asset = ScriptableObject.CreateInstance<Tile>();
        asset.sprite = sprite;
        asset.colliderType = Tile.ColliderType.None;
        tiles.Add(type, asset);
    }

    public void DrawTile(int x, int y)
    {
        DrawTile(x, y, initialTerrain);
    }

    public bool DrawTile(int x, int y, TerrainType type)
    {
        if (!ready || grid == null || !tiles.TryGetValue(type, out Tile asset)) return false;

        Vector2Int position = new Vector2Int(x, y);
        GridCell cell = grid.GetCell(position);
        if (cell.TerrainTile != null && cell.TerrainTile.Owner != this
            && cell.TerrainTile.Owner != null)
        {
            Debug.LogWarning("Another terrain renderer owns cell " + position, this);
            return false;
        }

        tilemap.SetTile(new Vector3Int(x, y, 0), asset);
        if (cell.TerrainTile == null || cell.TerrainTile.Owner != this)
        {
            cell.TerrainTile = new GridTile(position, type, this);
        }
        else
        {
            cell.TerrainTile.Type = type;
        }

        registeredPositions.Add(position);
        return true;
    }

    public bool TryHoe(Vector2Int position)
    {
        if (!ready || grid == null || !grid.TryGetCell(position, out GridCell cell)
            || cell.TerrainTile == null || cell.TerrainTile.Owner != this
            || cell.TerrainTile.Type != TerrainType.Dirt)
        {
            return false;
        }

        // Replace only this cell's asset; never modify a shared Tile's sprite.
        return DrawTile(position.x, position.y, TerrainType.TilledDirt);
    }

    public void RemoveTile(int x, int y)
    {
        Vector2Int position = new Vector2Int(x, y);
        if (!ready || grid == null || !grid.TryGetCell(position, out GridCell cell)
            || cell.TerrainTile == null || cell.TerrainTile.Owner != this) return;

        tilemap.SetTile(new Vector3Int(x, y, 0), null);
        cell.TerrainTile = null;
        registeredPositions.Remove(position);
    }

    private void OnDestroy()
    {
        if (grid != null)
        {
            foreach (Vector2Int position in registeredPositions)
            {
                if (grid.TryGetCell(position, out GridCell cell)
                    && cell.TerrainTile != null && cell.TerrainTile.Owner == this)
                {
                    cell.TerrainTile = null;
                    if (tilemap != null)
                    {
                        tilemap.SetTile(new Vector3Int(position.x, position.y, 0), null);
                    }
                }
            }
        }

        foreach (Tile asset in tiles.Values) Destroy(asset);
    }
}
