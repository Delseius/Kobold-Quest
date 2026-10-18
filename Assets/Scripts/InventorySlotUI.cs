using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple two-slot inventory display. Assign the kobold background sprite,
/// the two slot Images, and (optionally) the held-item icon Image in the Inspector.
/// The kobold image is drawn behind both slots.
/// </summary>
public class InventorySlotUI : MonoBehaviour
{
    [Header("Kobold Background")]
    [SerializeField] private Image koboldBackground;
    [SerializeField] private Sprite koboldBackgroundSprite;

    [Header("Two Inventory Slots")]
    [SerializeField] private Image firstSlot;
    [SerializeField] private Image secondSlot;
    [SerializeField] private Sprite emptySlotSprite;

    [Header("Optional Item Icons")]
    [SerializeField] private Image firstItemIcon;
    [SerializeField] private Image secondItemIcon;

    private void Awake()
    {
        if (koboldBackground != null && koboldBackgroundSprite != null)
        {
            koboldBackground.sprite = koboldBackgroundSprite;
            koboldBackground.preserveAspect = true;
            koboldBackground.raycastTarget = false;
            koboldBackground.transform.SetAsFirstSibling();
        }

        ConfigureSlot(firstSlot);
        ConfigureSlot(secondSlot);
        ClearSlot(firstItemIcon);
        ClearSlot(secondItemIcon);
    }

    private void ConfigureSlot(Image slot)
    {
        if (slot == null) return;

        if (emptySlotSprite != null)
            slot.sprite = emptySlotSprite;

        slot.preserveAspect = true;
    }

    /// <summary>Show an item icon in slot 1 or 2 (slotNumber is 1-based).</summary>
    public void SetSlot(int slotNumber, Sprite itemSprite)
    {
        Image icon = GetItemIcon(slotNumber);
        if (icon == null)
        {
            Debug.LogWarning("Assign the item icon Image for inventory slot " + slotNumber + ".");
            return;
        }

        icon.sprite = itemSprite;
        icon.enabled = itemSprite != null;
        icon.preserveAspect = true;
    }

    /// <summary>Clear slot 1 or 2.</summary>
    public void ClearSlot(int slotNumber)
    {
        ClearSlot(GetItemIcon(slotNumber));
    }

    private Image GetItemIcon(int slotNumber)
    {
        if (slotNumber == 1) return firstItemIcon;
        if (slotNumber == 2) return secondItemIcon;

        Debug.LogWarning("Inventory slot number must be 1 or 2.");
        return null;
    }

    private void ClearSlot(Image icon)
    {
        if (icon == null) return;
        icon.sprite = null;
        icon.enabled = false;
    }
}