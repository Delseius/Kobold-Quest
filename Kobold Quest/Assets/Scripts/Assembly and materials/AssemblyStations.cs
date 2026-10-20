using System.Collections.Generic;
//using System.Diagnostics;

//using System.Diagnostics;
using UnityEngine;

public class AssemblyStations : MonoBehaviour
{
    public Recipes RecipeCatalogue;
    //private GameObject[] AssemblyInventory;
    [SerializeField] private AssemblyInventorySystem AssemblyInventory;

    [SerializeField] private Transform FinalProduct;


    //Takes a recipe and checks if the required materials are available
    //Then returns int. Returns 1 if it can be crafted, returns 0 if it can't, anything else is an error
    public int CanCraft(Recipes.Recipe Recipe)
    {
        //Create tempoary recipe for checking
        Recipes.Recipe temp = Recipe;
        
        //Checks if the recipe has materials 
        if(temp.RecipeMaterials.Count == 0 || temp.RecipeMaterials == null ||
            temp.RecipeProduct == null)
        {
            Debug.LogError("The Recipe: ( " + temp.RecipeName + 
            " ) which produces ( " + temp.RecipeProduct + 
            " ) has no materials");
            return 2;
        }
        // check if recipe is unlocked 
        if (!temp.RecipeUnlocked)
        {
            Debug.Log("Recipe " + temp.RecipeName + " is locked.");
            return 0;
        }

        // check if has an AssemblyInventoryStation
        if (AssemblyInventory == null)
        {
            Debug.LogError(
                "AssemblyInventorySystem has not been assigned.");
            return 2;
        }

        // Track slots already used to satisfy a requirement.
        // make sure the slots match 
        List<int> matchedSlots = new List<int>();

        //Cycle through inventory
        /*foreach (GameObject item in AssemblyInventory)
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
                }
            }
            
        }*/

        // Check every required material.
        foreach (GameObject requiredMaterial in temp.RecipeMaterials)
        {
            if (requiredMaterial == null)
            {
                Debug.LogError(
                    "Recipe " + temp.RecipeName +
                    " contains a missing material reference.");
                return 2;
            }

            bool found = false;

            // AssemblyInventorySystem currently stores 15 items.
            for (int slot = 0; slot < 15; slot++)
            {
                if (matchedSlots.Contains(slot))
                    continue;

                GameObject item =
                    AssemblyInventory.GetAssemblyItem(slot);

                if (item == null)
                    continue;

                string itemName =
                    item.name.Replace("(Clone)", "").Trim();

                if (itemName == requiredMaterial.name)
                {
                    matchedSlots.Add(slot);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Debug.Log(
                    "Missing " + requiredMaterial.name +
                    " for recipe " + temp.RecipeName + ".");
                return 0;
            }
        }

        Debug.Log(temp.RecipeName + " can be crafted.");
        return 1;

        //Debug.LogError("Can not craft " + temp.RecipeName + ".");
        //return 0;
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

        // Record all required slots before consuming ingredients.
        List<int> matchedSlots = new List<int>();

        //Loop through all materials in the Assembly Station's inventory
        /*foreach (GameObject item in AssemblyInventory)
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
                }
            }
            
        }*/
        //Debug.LogError("Failed to craft " + temp.RecipeName + ".");
        //return 0;
        foreach (GameObject requiredMaterial in temp.RecipeMaterials)
        {
            bool found = false;

            for (int slot = 0; slot < 15; slot++)
            {
                if (matchedSlots.Contains(slot))
                    continue;

                GameObject item =
                    AssemblyInventory.GetAssemblyItem(slot);

                if (item == null)
                    continue;

                string itemName =
                    item.name.Replace("(Clone)", "").Trim();

                if (itemName == requiredMaterial.name)
                {
                    matchedSlots.Add(slot);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Debug.LogError(
                    "Ingredient check failed during crafting.");
                return 0;
            }
        }
        // Consume the matched ingredients and clear their UI slots.
        foreach (int slot in matchedSlots)
        {
            if (!AssemblyInventory.ConsumeAssemblyItem(slot))
            {
                Debug.LogError(
                    "Failed to consume ingredient in slot " +
                    (slot + 1) + ".");
                return 0;
            }
        }
        // Spawn the crafted product.
        Vector3 spawnPosition = FinalProduct != null
            ? FinalProduct.position
            : transform.position;

        Instantiate(
            temp.RecipeProduct,
            spawnPosition,
            Quaternion.identity);

        Debug.Log(temp.RecipeName + " crafted successfully!");
        return 1;
    }

}
