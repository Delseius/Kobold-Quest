using UnityEngine;

public class Hoe : Tool
{
    [Header("Tool Type")]
    [SerializeField]
    private ToolType toolType;

    [Header("Tool Toughness")]
    [SerializeField]
    public int toughness;

    [Header("Farm Plot")]
    [SerializeField]
    private GameObject farmPlotPrefab;

    /// <summary>
    /// Uses the hoe on the grid cell immediately
    /// in front of the kobold.
    /// </summary>
    public void hoeUse()
    {
        // Check durability.
        if (toolDurability <= 0)
        {
            Debug.Log("Hoe is broken.");
            return;
        }

        // Find the grid cell in front of the kobold.
        GridCell targetCell =
            CheckSpace();

        if (targetCell == null)
        {
            Debug.Log(
                "Hoe cannot be used: " +
                "there is no valid target cell."
            );

            return;
        }

        // Check that the target cell has terrain.
        if (targetCell.TerrainTile == null)
        {
            Debug.Log(
                "Hoe cannot be used on cell " +
                targetCell.Position +
                ": there is no terrain."
            );

            return;
        }

        // Ask the terrain whether it can be hoed.
        if (!targetCell.TerrainTile.TryHoe())
        {
            Debug.Log(
                "Hoe cannot be used on cell " +
                targetCell.Position +
                ": terrain rejected the action."
            );

            return;
        }

        // Convert the target grid position into
        // a world position.
        Vector3 spawnPosition =
            GridSpace.Instance.GridToWorld(
                targetCell.Position
            );

        // Spawn the farm plot.
        spawnFarmPlot(spawnPosition);

        // Reduce durability only after a successful use.
        toolDurability -= 1;

        Debug.Log(
            "Hoe successfully used on grid cell " +
            targetCell.Position
        );
    }

    /// <summary>
    /// Spawns the farm plot at the supplied world position.
    /// </summary>
    public virtual void spawnFarmPlot(
        Vector2 spawnPosition
    )
    {
        if (farmPlotPrefab == null)
        {
            Debug.LogError(
                "Cannot spawn farm plot: " +
                "Farm Plot Prefab has not been assigned."
            );

            return;
        }

        Instantiate(
            farmPlotPrefab,
            spawnPosition,
            Quaternion.identity
        );
    }
}