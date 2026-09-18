using UnityEngine;

public class Hammer : Tool
{
    [Header("Tool Type")]
    [SerializeField]
    ToolType toolType;
    [Header("Tool Toughness")]
    [SerializeField]
    public int toughness;
}
