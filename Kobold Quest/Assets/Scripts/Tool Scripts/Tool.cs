using UnityEngine;

public class Tool : MonoBehaviour
{
    [Header("Tool Durability")]
    [SerializeField]
    // protected static int y;
    public int toolDurability;
    public enum ToolType
    {
        Pickaxe,
        Shovel,
        Hoe,
        Hammer
    }

    public int checkSpace()
    {
        return 0;
    }
}
