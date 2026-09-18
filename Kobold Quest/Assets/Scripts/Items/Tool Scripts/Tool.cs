using UnityEngine;


public class Tool : MonoBehaviour
{
    [Header("Tool Durability")]
    [SerializeField]
    public int toolDurability;

    public enum ToolType
    {
        Pickaxe,
        Shovel,
        Hoe,
        Hammer
    }

    /// <summary>
    /// Finds the grid cell immediately in front of the kobold.
    /// The direction is determined by the kobold's facing direction.
    /// </summary>
    public GridCell CheckSpace()
    {
        if (GridSpace.Instance == null)
        {
            Debug.LogError(
                "Cannot check tool space: " +
                "GridSpace does not exist."
            );

            return null;
        }

        // Find the player/kobold.
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError(
                "Cannot check tool space: " +
                "No GameObject with the Player tag was found."
            );

            return null;
        }

        Transform playerTransform =
            player.transform;

        // Find the kobold's current grid position.
        Vector2Int koboldGridPosition =
            GridSpace.Instance.WorldToGrid(
                playerTransform.position
            );

        // Determine which direction the kobold is facing.
        Vector2Int facingDirection;

        if (playerTransform.localScale.x < 0)
        {
            facingDirection = Vector2Int.left;
        }
        else
        {
            facingDirection = Vector2Int.right;
        }

        // Find the grid cell immediately in front
        // of the kobold.
        Vector2Int targetGridPosition =
            koboldGridPosition +
            facingDirection;

        // Try to get the target cell.
        if (
            !GridSpace.Instance.TryGetCell(
                targetGridPosition,
                out GridCell targetCell
            )
        )
        {
            Debug.Log(
                "No registered grid cell at " +
                targetGridPosition
            );

            return null;
        }

        return targetCell;
    }

    /// <summary>
    /// Gets the world position of the grid cell
    /// immediately in front of the kobold.
    /// </summary>
    public Vector3 GetToolTargetPosition()
    {
        GridCell targetCell =
            CheckSpace();

        if (targetCell == null)
        {
            return transform.position;
        }

        return GridSpace.Instance.GridToWorld(
            targetCell.Position
        );
    }
}