using UnityEngine;

public class Hoe : Tool
{
   [Header("Tool Type")]
    [SerializeField]
    ToolType toolType;
    [Header("Tool Toughness")]
    [SerializeField]
    public int toughness;

    public void hoeUse()
    {
        if(toolDurability > 0) //check durability
        {
            //Code to make the block tilled
            toolDurability -= 1; //Reduce durability
        } else
        {
            //Show that hoe is broken
            return;
        }
    }
}
