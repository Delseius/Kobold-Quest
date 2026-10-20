//using System.Diagnostics;
using UnityEngine;

public class Assembly : MonoBehaviour
{
    [SerializeField] private AssemblyStations assemblyStation;
    [SerializeField] private Recipes recipeCatalogue;

    private int selectedRecipeIndex = -1;

    public void SelectRecipe(int recipeIndex)
    {
        if (recipeCatalogue == null ||
            recipeCatalogue.AllRecipes == null ||
            recipeIndex < 0 ||
            recipeIndex >= recipeCatalogue.AllRecipes.Length)
        {
            Debug.LogError("Invalid recipe selection.");
            return;
        }

        Recipes.Recipe recipe =
            recipeCatalogue.AllRecipes[recipeIndex];

        if (!recipe.RecipeUnlocked)
        {
            Debug.Log("This recipe is locked.");
            return;
        }

        selectedRecipeIndex = recipeIndex;
        Debug.Log("Selected recipe: " + recipe.RecipeName);
    }

    public void CraftSelectedRecipe()
    {
        if (assemblyStation == null || recipeCatalogue == null)
        {
            Debug.LogError("Assign AssemblyStations and Recipes.");
            return;
        }

        if (selectedRecipeIndex < 0)
        {
            Debug.Log("Select a recipe before crafting.");
            return;
        }

        Recipes.Recipe recipe =
            recipeCatalogue.AllRecipes[selectedRecipeIndex];

        assemblyStation.CraftItem(recipe);
    }
}
