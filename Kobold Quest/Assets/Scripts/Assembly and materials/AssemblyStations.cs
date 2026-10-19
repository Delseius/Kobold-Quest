using System.Diagnostics;
using UnityEngine;

public class AssemblyStations : MonoBehaviour
{
    [Header("Recipe Buttons")]
    [SerializeField] private Button recipe1Button;
    [SerializeField] private Button recipe2Button;
    [SerializeField] private Button recipe3Button;
    [SerializeField] private Button craftButton;

    [Header("Recipe Selection Flags")]
    public bool recipe1selected;
    public bool recipe2selected;
    public bool recipe3selected;

    [Header("Assembly Slots")]
    [SerializeField] private Transform assemblySlot1;
    [SerializeField] private Transform assemblySlot2;
    [SerializeField] private Transform assemblySlot3;

    [Header("Crafting Slot")]
    [SerializeField] private Transform craftingSlot;
    [SerializeField] private GameObject craftedObject;

    [Header("Recipe 1 Requirements")]
    [SerializeField] private string requiredItem1 = "iron_Pickaxe 1";
    [SerializeField] private string requiredItem2 = "iron_Pickaxe 2";

    [Header("Other Recipe Requirements")]
    [SerializeField] private string recipe2Item1 = "";
    [SerializeField] private string recipe2Item2 = "";
    [SerializeField] private string recipe3Item1 = "";
    [SerializeField] private string recipe3Item2 = "";

    [Header("Crafting Status")]
    public bool canBeCrafted;

    // sets
    private void Awake()
    {
        if (recipe1Button != null)
            recipe1Button.onClick.AddListener(SelectRecipe1);

        if (recipe2Button != null)
            recipe2Button.onClick.AddListener(SelectRecipe2);

        if (recipe3Button != null)
            recipe3Button.onClick.AddListener(SelectRecipe3);

        if (craftButton != null)
            craftButton.onClick.AddListener(CraftSelectedRecipe);
    }
    // reset all recipe 
    public void Start()
    {
        ResetRecipeSelection();
        canBeCrafted = false;
    }
    // shows the 
    public void SelectRecipe1()
    {

        ResetRecipeSelection();

        recipe1selected = true;

        Debug.Log("Recipe 1 selected.");
    }

    public void SelectRecipe2()
    {
        ResetRecipeSelection();

        recipe2selected = true;

        Debug.Log("Recipe 2 selected.");
    }

    public void SelectRecipe3()
    {
        ResetRecipeSelection();

        recipe3selected = true;

        Debug.Log("Recipe 3 selected.");
    }
    private void ResetRecipeSelection()
    {
        recipe1selected = false;
        recipe2selected = false;
        recipe3selected = false;
        canBeCrafted = false;
    }




}
