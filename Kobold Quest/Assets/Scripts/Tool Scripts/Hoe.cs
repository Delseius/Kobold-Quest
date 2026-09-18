using UnityEditor.SceneManagement;
using UnityEngine;

public class Hoe : Tool
{
   [Header("Tool Type")]
    [SerializeField]
    ToolType toolType;
    [Header("Tool Toughness")]
    [SerializeField]
    public int toughness;
    [SerializeField]
    private GameObject farmPlotPrefab;
    public void hoeUse()
    {
        if(toolDurability > 0) //check durability
        {
            //Code to check if it can be tilled
            if(checkSpace() == 1)
            {
                //spawnFarmPlot();
                toolDurability -= 1; //Reduce durability
            } else
            {
                
            }
            
        } else
        {
            //Show that hoe is broken
            return;
        }
    }
    
    public virtual void spawnFarmPlot(Vector2 spawnPosition)
    {
        //string path = "Assets/Prefabs/Farming/FarmPlot.prefab";
        Instantiate(farmPlotPrefab, spawnPosition, Quaternion.identity);
    }
}
