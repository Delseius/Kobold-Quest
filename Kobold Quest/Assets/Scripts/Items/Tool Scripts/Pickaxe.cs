using System.ComponentModel;
using UnityEngine;

public class Pickaxe : Tool
{
    [Header("Tool Type")]
    [SerializeField]
    ToolType toolType;
    [Header("Tool Toughness")]
    [SerializeField]
    public int toughness;
    
    public void pickaxeUse()
    {
        // Check durability.
        if (toolDurability <= 0)
        {
            Debug.Log("Pickaxe is broken.");
            //Destroy(gameObject);
            return;
        }

        // Find the grid cell in front of the kobold.
        GridCell targetCell =
            CheckSpace();

        if (targetCell == null)
        {
            Debug.Log(
                "Pickaxe cannot be used: " +
                "there is no valid target cell."
            );

            return;
        }

        // Check that the grid cell is a block that can be destroyed.


        // Make the block able to be picked up.
        

        // Reduce durability only after a successful use.
        toolDurability -= toughness;
        ToolDurabilityIndicator durabilityIndicator =
        GetComponent<ToolDurabilityIndicator>();

        if (durabilityIndicator != null)
        {
            durabilityIndicator.ShowIndicator();
        }

        Debug.Log(
            "Pickaxe successfully used on grid cell " +
            targetCell.Position
        );
    }

}
