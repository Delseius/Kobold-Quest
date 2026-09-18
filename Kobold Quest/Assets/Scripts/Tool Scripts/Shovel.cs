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

            toolDurability -= 1;
        }
        else
        {
            // Show that shovel is broken.
            return;
        }
    }
}
