//using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Inventory for Kobold Quest: a hand slot plus a list of inventory slots.
///
/// This script does NOT modify pickupobject, PickupInput, PickupMovement,
/// or PickupDropSystem. It uses their existing public interface:
/// - pickupobject.heldObject
/// - pickupobject.ClearHeldState()
/// - pickupobject.InitiatePickup()
///
/// Controls:
/// - I          : show / hide all inventory slots.
/// - Q          : hand item -> ORIGINAL slot (slot 0).
///                Empty hand -> take the item back from slot 0.
/// - Mouse hover: moving the mouse onto an inventory slot sends the hand
///                item to THAT slot only. With an empty hand it takes that
///                slot's item instead (can be turned off in the Inspector).
///                It triggers once per hover, when the mouse ENTERS the slot.
///
/// The UI lives in the scene hierarchy. Assign the slots in the Inspector.
/// </summary>
public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    [Header("Controls")]
    [SerializeField] private bool useQKey = true;
    [SerializeField] private bool useMouseHover = true;
    [Tooltip("If true, hovering a filled slot with an empty hand takes its item.")]
    [SerializeField] private bool hoverTakesItems = true;
    [Tooltip("Press this key to show/hide the inventory slots.")]
    [SerializeField] private Key toggleInventoryKey = Key.I;
    [Tooltip("If true, Q and hovering do nothing while the slots are hidden.")]
    [SerializeField] private bool requiresInventoryOpen = true;
    [SerializeField] private bool inventoryStartsVisible = false;

    [Header("Item Filter")]
    [Tooltip("If true, only objects with the tag below can be stored.")]
    [SerializeField] private bool onlyAllowTag = true;
    [Tooltip("Must match a tag in Edit > Project Settings > Tags and Layers (case sensitive).")]
    [SerializeField] private string allowedTag = "Equipment";

    [Header("UI (assign in Inspector)")]
    [SerializeField] private InventorySlotUI handSlot;
    [Tooltip("Element 0 is the ORIGINAL slot that Q uses.")]
    [SerializeField] private InventorySlotUI[] inventorySlots = new InventorySlotUI[2];

    private GameObject[] storedItems;

    // Hover tracking: actions happen when the mouse ENTERS a slot.
    private int hoveredSlot = -1;
    private bool slotsWereVisible;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        storedItems = new GameObject[inventorySlots.Length];

        if (handSlot == null)
        {
            Debug.LogError(
                "InventorySystem: Hand Slot is not assigned in the Inspector.",
                this
            );
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] == null)
            {
                Debug.LogError(
                    "InventorySystem: Inventory Slot " + i +
                    " is not assigned in the Inspector.",
                    this
                );
            }
        }
    }

    private void Start()
    {
        SetInventoryVisible(inventoryStartsVisible);
        RefreshUI();
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null)
        {
            if (keyboard[toggleInventoryKey].wasPressedThisFrame)
            {
                SetInventoryVisible(!IsInventoryVisible);
            }

            if (useQKey && keyboard.qKey.wasPressedThisFrame)
            {
                HandleQ();
            }
        }

        if (useMouseHover && Mouse.current != null)
        {
            HandleMouseHover(Mouse.current.position.ReadValue());
        }

        RefreshUI();
    }

    // ------------------------------------------------------------
    // Input handling
    // ------------------------------------------------------------

    private void HandleQ()
    {
        if (requiresInventoryOpen && !IsInventoryVisible)
        {
            Debug.Log("Open the inventory first.");
            return;
        }

        if (pickupobject.heldObject != null)
        {
            TryStoreHeldItemInSlot(0);
        }
        else
        {
            TryTakeFromSlot(0);
        }
    }

    private void HandleMouseHover(Vector2 screenPosition)
    {
        bool visible = IsInventoryVisible;
        int current = GetSlotIndexAt(screenPosition);

        // Just opened the inventory: remember where the mouse already is,
        // but don't act on it. Only a real move onto a slot counts.
        if (visible && !slotsWereVisible)
        {
            slotsWereVisible = true;
            hoveredSlot = current;
            return;
        }

        slotsWereVisible = visible;

        if (current == hoveredSlot)
        {
            return;
        }

        hoveredSlot = current;

        if (current >= 0)
        {
            HandleSlotHover(current); // only the hovered slot is affected
        }
    }

    /// <summary>
    /// True when the mouse is over a visible inventory slot.
    /// PickupInput uses this so world objects behind a slot ignore the click.
    /// </summary>
    public static bool IsPointerOverInventorySlot()
    {
        if (
            Instance == null ||
            Mouse.current == null
        )
        {
            return false;
        }

        return Instance.GetSlotIndexAt(
            Mouse.current.position.ReadValue()
        ) >= 0;
    }

    /// <summary>
    /// Returns the index of the visible slot under the screen position,
    /// or -1 if there is none.
    /// </summary>
    private int GetSlotIndexAt(Vector2 screenPosition)
    {
        if (requiresInventoryOpen && !IsInventoryVisible)
        {
            return -1;
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlotUI slot = inventorySlots[i];

            if (slot == null || !slot.IsVisible)
            {
                continue;
            }

            RectTransform rect = slot.GetComponent<RectTransform>();

            Canvas canvas = slot.GetComponentInParent<Canvas>();
            Camera cam = null;

            if (
                canvas != null &&
                canvas.renderMode != RenderMode.ScreenSpaceOverlay
            )
            {
                cam = canvas.worldCamera;
            }

            if (
                RectTransformUtility.RectangleContainsScreenPoint(
                    rect,
                    screenPosition,
                    cam
                )
            )
            {
                return i;
            }
        }

        return -1;
    }

    private void HandleSlotHover(int index)
    {
        if (pickupobject.heldObject != null)
        {
            TryStoreHeldItemInSlot(index);
        }
        else if (hoverTakesItems)
        {
            TryTakeFromSlot(index);
        }
    }

    // ------------------------------------------------------------
    // Public API
    // ------------------------------------------------------------

    public bool IsInventoryVisible
    {
        get
        {
            foreach (InventorySlotUI slot in inventorySlots)
            {
                if (slot != null)
                {
                    return slot.IsVisible;
                }
            }

            return false;
        }
    }

    // Uses a CanvasGroup (alpha) instead of SetActive, so the slots keep
    // their place in the hierarchy and nothing else in the layout moves.
    public void SetInventoryVisible(bool visible)
    {
        foreach (InventorySlotUI slot in inventorySlots)
        {
            if (slot != null)
            {
                slot.SetVisible(visible);
            }
        }
    }

    public bool HasInventoryItem()
    {
        foreach (GameObject item in storedItems)
        {
            if (item != null)
            {
                return true;
            }
        }

        return false;
    }

    public bool HasInventoryItem(int index)
    {
        return IsValidSlot(index) && storedItems[index] != null;
    }

    public GameObject GetInventoryItem(int index = 0)
    {
        return IsValidSlot(index) ? storedItems[index] : null;
    }

    public bool TryStoreHeldItem()
    {
        return TryStoreHeldItemInSlot(0);
    }

    public bool TryTakeInventoryItem()
    {
        return TryTakeFromSlot(0);
    }

    /// <summary>
    /// Stores the object currently handled by the original pickup system
    /// into one specific inventory slot.
    /// </summary>
    public bool TryStoreHeldItemInSlot(int index)
    {
        if (!IsValidSlot(index))
        {
            return false;
        }

        if (storedItems[index] != null)
        {
            Debug.Log("Inventory slot " + index + " is already occupied.");
            return false;
        }

        GameObject held = pickupobject.heldObject;

        if (held == null)
        {
            Debug.Log("There is no item in the hand to store.");
            return false;
        }

        // Only items with the allowed tag may enter an inventory slot.
        // (held.tag is compared directly so an undefined tag can't throw.)
        if (onlyAllowTag && held.tag != allowedTag)
        {
            Debug.Log(
                held.name +
                " can't be stored. Only objects tagged '" +
                allowedTag +
                "' are allowed (this one is '" +
                held.tag +
                "')."
            );

            return false;
        }

        pickupobject heldPickup = held.GetComponent<pickupobject>();

        if (heldPickup == null)
        {
            Debug.LogWarning(
                held.name +
                " cannot be stored because it has no pickupobject script."
            );

            return false;
        }

        // Remember the object before ClearHeldState clears the static reference.
        storedItems[index] = held;

        heldPickup.ClearHeldState();
        held.SetActive(false);

        Debug.Log(
            held.name +
            " moved from the hand slot to inventory slot " + index + "."
        );

        RefreshUI();
        return true;
    }

    /// <summary>
    /// Returns one slot's item to the original pickup system.
    /// </summary>
    public bool TryTakeFromSlot(int index)
    {
        if (!IsValidSlot(index))
        {
            return false;
        }

        if (pickupobject.heldObject != null)
        {
            Debug.Log("The hand slot is already occupied.");
            return false;
        }

        GameObject item = storedItems[index];

        if (item == null)
        {
            Debug.Log("Inventory slot " + index + " is empty.");
            return false;
        }

        pickupobject itemPickup = item.GetComponent<pickupobject>();

        if (itemPickup == null)
        {
            Debug.LogWarning(
                item.name +
                " cannot be returned because it has no pickupobject script."
            );

            storedItems[index] = null;
            RefreshUI();
            return false;
        }

        // The item was disabled wherever the hand was when it was stored.
        // Bring it back to the player first so it doesn't fly across the map.
        Transform player = itemPickup.PlayerTransformReference;

        if (player == null)
        {
            player = pickupobject.FindPlayerTransform();
        }

        if (player != null)
        {
            item.transform.position = new Vector3(
                player.position.x,
                player.position.y,
                item.transform.position.z
            );
        }

        item.SetActive(true);
        itemPickup.InitiatePickup();

        storedItems[index] = null;

        Debug.Log(
            item.name +
            " moved from inventory slot " + index + " back to the hand."
        );

        RefreshUI();
        return true;
    }

    // ------------------------------------------------------------
    // UI
    // ------------------------------------------------------------

    private bool IsValidSlot(int index)
    {
        return
            storedItems != null &&
            index >= 0 &&
            index < storedItems.Length;
    }

    private void RefreshUI()
    {
        if (handSlot != null)
        {
            handSlot.SetSprite(
                GetSpriteFromObject(pickupobject.heldObject)
            );
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] != null)
            {
                inventorySlots[i].SetSprite(
                    GetSpriteFromObject(storedItems[i])
                );
            }
        }
    }

    private Sprite GetSpriteFromObject(GameObject objectToRead)
    {
        if (objectToRead == null)
        {
            return null;
        }

        SpriteRenderer renderer = objectToRead.GetComponent<SpriteRenderer>();

        if (renderer != null)
        {
            return renderer.sprite;
        }

        SpriteRenderer childRenderer =
            objectToRead.GetComponentInChildren<SpriteRenderer>();

        if (childRenderer != null)
        {
            return childRenderer.sprite;
        }

        return null;
    }
}