using UnityEngine;

public class GridTest : MonoBehaviour
{
    private void Start()
    {
        if (GridSpace.Instance == null)
        {
            Debug.LogError(
                "GridTest could not find a GridSpace in the scene."
            );

            return;
        }

        Vector2Int gridPosition =
            GridSpace.Instance.WorldToGrid(transform.position);

        Vector3 cellCentre =
            GridSpace.Instance.GridToWorld(gridPosition);

        Debug.Log(
            "World Position: " + transform.position
        );

        Debug.Log(
            "Grid Position: " + gridPosition
        );

        Debug.Log(
            "Cell Centre: " + cellCentre
        );
    }
}