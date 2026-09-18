using UnityEngine;

public abstract class GridObject : MonoBehaviour
{
    [SerializeField]
    private GridObjectType objectType = GridObjectType.WorldObject;

    public Vector2Int GridPosition { get; private set; }

    protected virtual void Start()
    {
        RegisterWithGrid();
    }

    protected virtual void RegisterWithGrid()
    {
        if (GridSpace.Instance == null)
        {
            Debug.LogError("No GridSpace exists in the scene.");
            return;
        }

        GridPosition =
            GridSpace.Instance.WorldToGrid(transform.position);

        GridCell cell =
            GridSpace.Instance.GetCell(GridPosition);

        switch (objectType)
        {
            case GridObjectType.WorldObject:
                cell.WorldObject = gameObject;
                break;

            case GridObjectType.Block:
                cell.Block = gameObject;
                break;

            case GridObjectType.Item:
                cell.Item = gameObject;
                break;

            case GridObjectType.Tile:
                cell.Tile = gameObject;
                break;
        }
    }

    protected virtual void UnregisterFromGrid()
    {
        if (GridSpace.Instance == null)
        {
            return;
        }

        GridCell cell =
            GridSpace.Instance.GetCell(GridPosition);

        switch (objectType)
        {
            case GridObjectType.WorldObject:

                if (cell.WorldObject == gameObject)
                {
                    cell.WorldObject = null;
                }

                break;

            case GridObjectType.Block:

                if (cell.Block == gameObject)
                {
                    cell.Block = null;
                }

                break;

            case GridObjectType.Item:

                if (cell.Item == gameObject)
                {
                    cell.Item = null;
                }

                break;
            case GridObjectType.Tile:
                if (cell.Tile == gameObject)
                {
                    cell.Tile = null;
                }
                break;
        }
    }

    protected virtual void OnDestroy()
    {
        UnregisterFromGrid();
    }

    public void SnapToGrid()
    {
        if (GridSpace.Instance == null)
        {
            Debug.LogError("No GridSpace exists in the scene.");
            return;
        }

        UnregisterFromGrid();

        GridPosition =
            GridSpace.Instance.WorldToGrid(transform.position);

        transform.position =
            GridSpace.Instance.GridToWorld(GridPosition);

        RegisterWithGrid();
    }

    public Vector3 GetGridWorldPosition()
    {
        if (GridSpace.Instance == null)
        {
            return transform.position;
        }

        return GridSpace.Instance.GridToWorld(GridPosition);
    }
}