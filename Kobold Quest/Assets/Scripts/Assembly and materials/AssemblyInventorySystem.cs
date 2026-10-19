<<<<<<< HEAD

//using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

=======
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Separate four-slot storage for the assembly UI.
/// Assign the four AssemblySlotUI components in the Inspector.
/// The assembly slots accept objects with any tag listed in allowedTags.
/// InventorySystem remains separate and filters its own slots to Equipment.
/// </summary>
>>>>>>> 31de55aaedbb24990b1982304a988da03d21834f
public class AssemblyInventorySystem : MonoBehaviour
{
    [Header("Assembly UI")]
    [SerializeField] private GameObject assemblyUIPanel;
<<<<<<< HEAD

    [Header("Assembly Slots - Assign Exactly Four")]
    [SerializeField]
    private InventorySlotUI[] assemblySlots =
        new InventorySlotUI[4];

    [Header("Allowed Object Tags")]
    [SerializeField]
    private string[] allowedTags =
        { "Item", "Equipment" };

    [Header("Settings")]
    [SerializeField] private bool assemblyStartsVisible = false;

    private readonly GameObject[] storedItems = new GameObject[4];

    private pickupobject pickup;

    private void Awake()
    {
        // This script and pickupobject should be on the same GameObject.
        pickup = GetComponent<pickupobject>();

        if (pickup == null)
        {
            Debug.LogError(
                "AssemblyInventorySystem requires pickupobject " +
                "on the same GameObject.",
                this
            );
        }
=======
    [SerializeField] private InventorySlotUI[] assemblySlots = new InventorySlotUI[4];

    [Header("Allowed item tags")]
    [Tooltip("Tags allowed in assembly slots. Add your project tag names here.")]
    [SerializeField] private string[] allowedTags = { "Items", "Item", "Equipment" };

    [Header("Controls")]
    [SerializeField] private bool useMouseHover = true;
    [SerializeField] private bool assemblyStartsVisible = false;

    private GameObject[] storedItems;
    private int hoveredSlot = -1;
    private bool wasVisible;

    public bool IsAssemblyVisible =>
        assemblyUIPanel != null && assemblyUIPanel.activeInHierarchy;

    private void Awake()
    {
        if (assemblySlots == null || assemblySlots.Length != 4)
        {
            Debug.LogError("AssemblyInventorySystem requires exactly four assembly slots.", this);
            assemblySlots = assemblySlots ?? new InventorySlotUI[4];
        }

        storedItems = new GameObject[assemblySlots.Length];
>>>>>>> 31de55aaedbb24990b1982304a988da03d21834f
    }

    private void Start()
    {
<<<<<<< HEAD
        if (assemblySlots == null || assemblySlots.Length != 4)
        {
            Debug.LogError(
                "AssemblyInventorySystem requires exactly four slots.",
                this
            );
            return;
        }

        if (assemblyUIPanel != null)
        {
            assemblyUIPanel.SetActive(assemblyStartsVisible);
        }
        else
        {
            Debug.LogError(
                "Assign KoboldAssemblystations to Assembly UI Panel.",
                this
            );
        }

        RefreshSlots();
=======
        if (assemblyUIPanel != null)
            assemblyUIPanel.SetActive(assemblyStartsVisible);

        RefreshUI();
>>>>>>> 31de55aaedbb24990b1982304a988da03d21834f
    }

    private void Update()
    {
<<<<<<< HEAD
        if (assemblyUIPanel == null ||
            !assemblyUIPanel.activeInHierarchy)
        {
            return;
        }

        if (assemblySlots == null ||
            Mouse.current == null ||
            !Mouse.current.leftButton.wasPressedThisFrame)
        {
            return;
        }

        for (int i = 0; i < assemblySlots.Length; i++)
        {
            InventorySlotUI slot = assemblySlots[i];

            if (slot == null)
                continue;

            if (IsMouseOverSlot(slot))
            {
                HandleSlotClick(i);
                return;
            }
        }
    }

    private bool IsMouseOverSlot(InventorySlotUI slot)
    {
        if (Mouse.current == null)
            return false;

        RectTransform rect = slot.GetComponent<RectTransform>();

        if (rect == null)
            return false;

        Canvas canvas = rect.GetComponentInParent<Canvas>();
        Camera uiCamera = null;

        if (canvas != null &&
            canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = canvas.worldCamera;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        return RectTransformUtility.RectangleContainsScreenPoint(
            rect,
            mousePosition,
            uiCamera
        );
    }

    private void HandleSlotClick(int slotIndex)
    {
        if (pickup == null)
            return;

        GameObject heldItem = pickupobject.heldObject;

        // If this slot contains an item, try to retrieve it.
        if (storedItems[slotIndex] != null)
        {
            if (heldItem == null)
            {
                TakeItemFromSlot(slotIndex);
            }

            return;
        }

        // If the slot is empty, try to store the held item.
        if (heldItem != null)
        {
            StoreHeldItemInSlot(slotIndex, heldItem);
        }
    }

    private bool HasAllowedTag(GameObject item)
    {
        if (item == null || allowedTags == null)
            return false;

        foreach (string allowedTag in allowedTags)
        {
            if (!string.IsNullOrEmpty(allowedTag) &&
                item.tag == allowedTag)
            {
                return true;
            }
        }

        return false;
    }

    public bool StoreHeldItemInSlot(int slotIndex, GameObject item)
    {
        if (pickup == null)
            return false;

        if (slotIndex < 0 || slotIndex >= storedItems.Length)
            return false;

        if (item == null || storedItems[slotIndex] != null)
            return false;

        if (!HasAllowedTag(item))
        {
            Debug.Log(
                item.name +
                " cannot be stored. Allowed tags: Item, Equipment."
            );
            return false;
        }

        // Only store the object currently held by the pickup system.
        if (pickupobject.heldObject != item)
            return false;

        // Clear the held state before storing the object.
        pickup.ClearHeldState();

        storedItems[slotIndex] = item;

        // Hide the item in the world while stored.
        item.SetActive(false);

        RefreshSlot(slotIndex);

        Debug.Log(
            item.name + " stored in assembly slot " +
            (slotIndex + 1) + "."
        );

        return true;
    }

    public bool TakeItemFromSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= storedItems.Length)
            return false;

        GameObject item = storedItems[slotIndex];

        if (item == null)
            return false;

        if (pickup == null || pickupobject.heldObject != null)
            return false;

        // Remove the item from the assembly slot.
        storedItems[slotIndex] = null;

        // Make the item visible in the scene again.
        item.SetActive(true);

        RefreshSlot(slotIndex);

        Debug.Log(
            item.name + " taken from assembly slot " +
            (slotIndex + 1) + "."
        );

        return true;
    }

    public GameObject GetAssemblyItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= storedItems.Length)
            return null;

        return storedItems[slotIndex];
    }

    public bool HasAssemblyItem(int slotIndex)
    {
        return GetAssemblyItem(slotIndex) != null;
    }

    public bool IsSlotOccupied(int slotIndex)
    {
        return HasAssemblyItem(slotIndex);
    }

    private void RefreshSlots()
    {
        if (assemblySlots == null)
            return;

        for (int i = 0; i < assemblySlots.Length; i++)
        {
            RefreshSlot(i);
        }
    }

    private void RefreshSlot(int slotIndex)
    {
        if (assemblySlots == null ||
            slotIndex < 0 ||
            slotIndex >= assemblySlots.Length)
        {
            return;
        }

        InventorySlotUI slot = assemblySlots[slotIndex];

        if (slot == null)
            return;

        GameObject item = storedItems[slotIndex];

        if (item == null)
        {
            slot.SetSprite(null);
        }
        else
        {
            SpriteRenderer spriteRenderer =
                item.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                slot.SetSprite(spriteRenderer.sprite);
            }
            else
            {
                slot.SetSprite(null);
            }
        }

        slot.SetVisible(true);
=======
        bool visible = IsAssemblyVisible;

        if (!visible)
        {
            hoveredSlot = -1;
            wasVisible = false;
            return;
        }

        if (useMouseHover && Mouse.current != null)
        {
            Vector2 position = Mouse.current.position.ReadValue();
            int current = GetSlotAt(position);

            // Avoid transferring an item merely because the panel opened under the pointer.
            if (!wasVisible)
            {
                wasVisible = true;
                hoveredSlot = current;
            }
            else if (current != hoveredSlot)
            {
                hoveredSlot = current;
                if (current >= 0)
                    HandleSlot(current);
            }
        }

        RefreshUI();
>>>>>>> 31de55aaedbb24990b1982304a988da03d21834f
    }

    public void SetAssemblyVisible(bool visible)
    {
        if (assemblyUIPanel != null)
<<<<<<< HEAD
        {
            assemblyUIPanel.SetActive(visible);
        }
    }
}

=======
            assemblyUIPanel.SetActive(visible);

        wasVisible = false;
        hoveredSlot = -1;
        RefreshUI();
    }

    public GameObject GetAssemblyItem(int index)
    {
        return IsValidIndex(index) ? storedItems[index] : null;
    }

    public bool HasAssemblyItem(int index)
    {
        return GetAssemblyItem(index) != null;
    }

    public bool TryStoreHeldItemInSlot(int index)
    {
        if (!IsValidIndex(index) || storedItems[index] != null)
            return false;

        GameObject held = pickupobject.heldObject;
        if (held == null)
        {
            Debug.Log("There is no held item to place in the assembly slot.");
            return false;
        }

        if (!HasAllowedTag(held))
        {
            Debug.Log(held.name + " cannot be placed in an assembly slot. Allowed tags: " +
                      string.Join(", ", allowedTags) + ".");
            return false;
        }

        pickupobject pickup = held.GetComponent<pickupobject>();
        if (pickup == null)
        {
            Debug.LogWarning(held.name + " has no pickupobject component.", held);
            return false;
        }

        storedItems[index] = held;
        pickup.ClearHeldState();
        held.SetActive(false);
        RefreshUI();

        Debug.Log(held.name + " stored in assembly slot " + (index + 1) + ".");
        return true;
    }

    public bool TryTakeFromSlot(int index)
    {
        if (!IsValidIndex(index) || storedItems[index] == null)
            return false;

        if (pickupobject.heldObject != null)
        {
            Debug.Log("The hand is already holding an item.");
            return false;
        }

        GameObject item = storedItems[index];
        pickupobject pickup = item.GetComponent<pickupobject>();
        if (pickup == null)
        {
            Debug.LogWarning(item.name + " has no pickupobject component.", item);
            return false;
        }

        Transform player = pickup.PlayerTransformReference;
        if (player == null)
            player = pickupobject.FindPlayerTransform();

        if (player != null)
            item.transform.position = new Vector3(player.position.x, player.position.y, item.transform.position.z);

        item.SetActive(true);
        pickup.InitiatePickup();
        storedItems[index] = null;
        RefreshUI();

        Debug.Log(item.name + " returned from assembly slot " + (index + 1) + " to the hand.");
        return true;
    }

    private void HandleSlot(int index)
    {
        if (pickupobject.heldObject != null)
            TryStoreHeldItemInSlot(index);
        else
            TryTakeFromSlot(index);
    }

    private bool HasAllowedTag(GameObject item)
    {
        if (allowedTags == null || allowedTags.Length == 0)
            return false;

        foreach (string allowedTag in allowedTags)
        {
            if (!string.IsNullOrWhiteSpace(allowedTag) && item.tag == allowedTag)
                return true;
        }

        return false;
    }

    private int GetSlotAt(Vector2 screenPosition)
    {
        for (int i = 0; i < assemblySlots.Length; i++)
        {
            InventorySlotUI slot = assemblySlots[i];
            if (slot == null || !slot.IsVisible)
                continue;

            RectTransform rect = slot.GetComponent<RectTransform>();
            Canvas canvas = slot.GetComponentInParent<Canvas>();
            Camera cam = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? canvas.worldCamera : null;

            if (rect != null &&
                RectTransformUtility.RectangleContainsScreenPoint(rect, screenPosition, cam))
                return i;
        }

        return -1;
    }

    private bool IsValidIndex(int index)
    {
        return storedItems != null && index >= 0 && index < storedItems.Length;
    }

    private void RefreshUI()
    {
        if (assemblySlots == null || storedItems == null)
            return;

        for (int i = 0; i < assemblySlots.Length; i++)
        {
            if (assemblySlots[i] != null)
                assemblySlots[i].SetSprite(GetSprite(storedItems[i]));
        }
    }

    private Sprite GetSprite(GameObject item)
    {
        if (item == null)
            return null;

        SpriteRenderer renderer = item.GetComponent<SpriteRenderer>();
        if (renderer == null)
            renderer = item.GetComponentInChildren<SpriteRenderer>();

        return renderer != null ? renderer.sprite : null;
    }
}
>>>>>>> 31de55aaedbb24990b1982304a988da03d21834f
