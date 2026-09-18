using System.ComponentModel;
using UnityEngine;

public class Pickaxe : Tool, IUsable
{
    [Header("Tool Type")]
    [SerializeField]
    ToolType toolType;
    [Header("Tool Toughness")]
    [SerializeField]
    public int toughness;
    
    public bool Use(GameObject user)
    {
        // Check durability.
        if (toolDurability <= 0)
        {
            Debug.Log("Pickaxe is broken.");
            return false;
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

            return false;
        }

        // pickupobject block = .GetComponent<pickupobject>();

        // // Check that the grid cell is a block that can be destroyed.
        

        // // Make the block able to be picked up.
        // pickupobject.SelectBlock(block);
        // block.UnlockBlockForPickup();

        // Reduce durability only after a successful use.
        toolDurability -= toughness;

        Debug.Log(
            "Pickaxe successfully used on grid cell " +
            targetCell.Position
        );
        return true;
    }

}
