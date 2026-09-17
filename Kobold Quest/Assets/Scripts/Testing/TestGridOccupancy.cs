using UnityEngine;
using UnityEngine.InputSystem;

public class TestGridOccupancy : MonoBehaviour
{
    [SerializeField] private GridObject testObject;

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TestOccupancy();
        }
    }

    private void TestOccupancy()
    {
        if (GridSpace.Instance == null)
        {
            Debug.LogError("GridSpace does not exist.");
            return;
        }

        Vector2Int gridPosition =
            GridSpace.Instance.WorldToGrid(testObject.transform.position);

        GridCell cell =
            GridSpace.Instance.GetCell(gridPosition);

        if (cell.Block != null)
        {
            Debug.Log(
                "Cell " + gridPosition +
                " contains block: " +
                cell.Block.name
            );
        }
        else
        {
            Debug.Log(
                "Cell " + gridPosition +
                " contains no block."
            );
        }
    }
}