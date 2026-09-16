using UnityEngine;

public abstract class GridObject : MonoBehaviour
{
    public Vector2Int GridPosition { get; private set; }

    protected virtual void Start()
    {
        RegisterWithGrid();
    }

    protected virtual void RegisterWithGrid()
    {
        if (GridSpace.Instance == null)
        {
            Debug.LogError(
                "No GridSpace exists in the scene."
            );

            return;
        }

        GridPosition =
            GridSpace.Instance.WorldToGrid(transform.position);
    }

    public void SnapToGrid()
    {
        if (GridSpace.Instance == null)
        {
            Debug.LogError(
                "No GridSpace exists in the scene."
            );

            return;
        }

        GridPosition =
            GridSpace.Instance.WorldToGrid(transform.position);

        transform.position =
            GridSpace.Instance.GridToWorld(GridPosition);
    }

    public Vector3 GetGridWorldPosition()
    {
        return GridSpace.Instance.GridToWorld(GridPosition);
    }
}