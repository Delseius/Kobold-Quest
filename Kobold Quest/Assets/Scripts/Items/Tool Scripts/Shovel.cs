using UnityEngine;

public class Shovel : Tool
{
    [Header("Tool Type")]
    [SerializeField]
    ToolType toolType;

    [Header("Tool Toughness")]
    [SerializeField]
    public int toughness;

    public void shovelUse()
    {
        if (toolDurability > 0)
        {
            // Code to make the block loose
            // or destroy the loose block.

            toolDurability -= toughness;
            
            //Check Durability
            if (toolDurability <= 0)
            {
                Debug.Log("Shovel is broken.");
                Destroy(gameObject);
                return;
            }

        }
        else
        {
            // Show that shovel is broken.
            return;
        }
    }
}
