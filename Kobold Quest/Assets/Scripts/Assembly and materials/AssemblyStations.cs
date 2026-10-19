using UnityEngine;

public class AssemblyStations : MonoBehaviour
{
    public Recipes RecipeCatalogue;
    private GameObject[] AssemblyInventory;
    
    
    //Takes a recipe and checks if the required materials are available
    //Then returns int. Returns 1 if it can be crafted, returns 0 if it can't, anything else is an error
    public int CanCraft(Recipes.Recipe Recipe)
    {
        //Create tempoary recipe for checking
        Recipes.Recipe temp = Recipe;
        
        //Checks if the recipe has materials 
        if(temp.RecipeMaterials.Count == 0 || temp.RecipeMaterials == null)
        {
            Debug.LogError("The Recipe: ( " + temp.RecipeName + 
            " ) which produces ( " + temp.RecipeProduct + 
            " ) has no materials");
            return 2;
        }
        //Cycle through inventory
        foreach(GameObject item in AssemblyInventory)
        {
            //Check if recipe list is empty
            if(temp.RecipeMaterials.Count == 0 || temp.RecipeMaterials == null) {
                Debug.LogError(temp.RecipeName + " can be crafted.");
                return 1;
            }

            //For every recipe material listed compare it to the material from the inventory
            for(int i = 0; i < temp.RecipeMaterials.Count; i++)
            {
                //compare item to required materials
                /*if( compare here) {
                    ////if item matches a material remove the material from the recipe list
                    //temp.RecipeMaterials.RemoveAt(i);
                    //break;
                }*/
            }
            
        }
        
        Debug.LogError("Can not craft " + temp.RecipeName + ".");
        return 0;
    }

    //Craft Recipe
    //Returns 0 if it fails to craft, returns 1 if it crafts successfully
    public int CraftItem(Recipes.Recipe Recipe)
    {
        Recipes.Recipe temp = Recipe;
        if(CanCraft(temp) != 1)
        {
            Debug.LogError("Can not craft " + temp.RecipeName + ".");
            return 0;
        }

        //Loop through all materials in the Assembly Station's inventory
        foreach(GameObject item in AssemblyInventory)
        {
            //Check if recipe list is empty
            if(temp.RecipeMaterials.Count == 0 || temp.RecipeMaterials == null) {
                //Spawn item. The object will be temp.RecipeProduct

                Debug.LogError(temp.RecipeName + " crafted successfully.");
                return 1;
            }

            //For every recipe material listed compare it to the material from the inventory
            for(int i = 0; i < temp.RecipeMaterials.Count; i++)
            {
                //compare material from Assembly inventory to recipe materials
                /*if( compare here) {
                    ////if item matches a material remove the material from the recipe list
                    ////Remove material from slot here
                    
                    //temp.RecipeMaterials.RemoveAt(i);
                    //break;
                }*/
            }
            
        }
        Debug.LogError("Failed to craft " + temp.RecipeName + ".");
        return 0;
    }

}
