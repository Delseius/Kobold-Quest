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



}
