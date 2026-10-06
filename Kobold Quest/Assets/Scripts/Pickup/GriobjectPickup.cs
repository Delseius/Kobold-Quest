//using System.Diagnostics;
//using System.Diagnostics;
using UnityEngine;

public class GriobjectPickup : MonoBehaviour
{
    private GridObject gridObject;

    private void Awake()
    {
        gridObject = GetComponent<GridObject>();

        if (gridObject == null)
        {
            Debug.LogError(
                gameObject.name +
                " needs a GridObject component for TestGridObject to work."
            );
        }
    }

    public void TestSnapToGrid()
    {
        if (gridObject == null)
        {
            Debug.LogError(
                gameObject.name +
                ": No GridObject found."
            );

            return;
        }

        if (GridSpace.Instance == null)
        {
            Debug.LogError(
                gameObject.name +
                ": No GridSpace exists in the scene."
            );

            return;
        }

        // Get the block's position before snapping.
        Vector3 beforePosition = transform.position;

        // Find which grid cell the block is currently over.
        Vector2Int gridPosition =
            GridSpace.Instance.WorldToGrid(beforePosition);

        Debug.Log(
            "[TestGridObject] Block drop detected\n" +
            "Block: " + gameObject.name + "\n" +
            "World Position Before: " + beforePosition + "\n" +
            "Grid Coordinates: " + gridPosition
        );

        // Use the existing GridObject snapping system.
        gridObject.SnapToGrid();

        // Get the final snapped position.
        Vector3 afterPosition = transform.position;

        Debug.Log(
            "[TestGridObject] Block snapped to grid\n" +
            "Block: " + gameObject.name + "\n" +
            "Grid Coordinates: " + gridObject.GridPosition + "\n" +
            "World Position After: " + afterPosition
        );

        // Check what is registered in this grid cell.
        GridSpace.Instance.DebugCell(gridObject.GridPosition);
    }
    
}
