
using System;
using System.Collections.Generic;
using UnityEngine;

public class Recipes : MonoBehaviour
{
    [Serializable]
    public struct Recipe
    {
        //Recipe Name
        public string RecipeName;
        //Array of materials required for crafting
        public List<GameObject> RecipeMaterials;
        //What object is crafted
        public GameObject RecipeProduct;
        //If the recipe is unlocked it can be used
        public bool RecipeUnlocked;
    }

    // All recipes in the game are stored here.
    public Recipe[] AllRecipes;
}


