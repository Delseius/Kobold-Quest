//using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Separate one-slot inventory system for Kobold Quest.
///
/// This script does NOT modify pickupobject, PickupInput, PickupMovement,
/// or PickupDropSystem. It uses their existing public interface:
/// - pickupobject.heldObject
/// - pickupobject.ClearHeldState()
/// - pickupobject.InitiatePickup()
///
/// Q:
/// - If the hand contains an item and the inventory is empty, store it.
/// - If the hand is empty and the inventory contains an item, retrieve it.
///
/// This script also contains the hand-slot display that used to live in
/// HandSlotSystem, so HandSlotSystem is no longer needed.
/// The UI now lives in the scene hierarchy. Assign the two slot
/// references in the Inspector.
/// </summary>
public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    [Header("Controls")]
    [SerializeField] private bool useQKey = true;
    [Tooltip("Press this key to show/hide the inventory slot.")]
    [SerializeField] private Key toggleInventoryKey = Key.I;
    [Tooltip("If true, Q does nothing while the inventory slot is hidden.")]
    [SerializeField] private bool qRequiresInventoryOpen = true;
    [SerializeField] private bool inventoryStartsVisible = false;

    [Header("UI (assign in Inspector)")]
    [SerializeField] private InventorySlotUI handSlot;
    [SerializeField] private InventorySlotUI inventorySlot;

    private GameObject inventoryObject;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (handSlot == null)
        {
            Debug.LogError(
                "InventorySystem: Hand Slot is not assigned in the Inspector.",
                this
            );
        }

        if (inventorySlot == null)
        {
            Debug.LogError(
                "InventorySystem: Inventory Slot is not assigned in the Inspector.",
                this
            );
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
            // Show / hide the inventory slot.
            if (keyboard[toggleInventoryKey].wasPressedThisFrame)
            {
                SetInventoryVisible(!IsInventoryVisible);
            }

            // Move items between the hand and the inventory slot.
            if (useQKey && keyboard.qKey.wasPressedThisFrame)
            {
                if (qRequiresInventoryOpen && !IsInventoryVisible)
                {
                    Debug.Log("Open the inventory first.");
                }
                else if (pickupobject.heldObject != null)
                {
                    TryStoreHeldItem();
                }
                else
                {
                    TryTakeInventoryItem();
                }
            }
        }

        RefreshUI();
    }

    public bool IsInventoryVisible =>
        inventorySlot != null && inventorySlot.IsVisible;

    // Uses a CanvasGroup (alpha) instead of SetActive, so the slot keeps
    // its place in the hierarchy and nothing else in the layout moves.
    public void SetInventoryVisible(bool visible)
    {
        if (inventorySlot != null)
        {
            inventorySlot.SetVisible(visible);
        }
    }

    public bool HasInventoryItem()
    {
        return inventoryObject != null;
    }

    public GameObject GetInventoryItem()
    {
        return inventoryObject;
    }

    public bool TryStoreHeldItem()
    {
        if (inventoryObject != null)
        {
            Debug.Log("Inventory slot is already occupied.");
            return false;
        }

        GameObject held = pickupobject.heldObject;

        if (held == null)
        {
            Debug.Log("There is no item in the hand to store.");
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
        inventoryObject = held;

        heldPickup.ClearHeldState();
        inventoryObject.SetActive(false);

        Debug.Log(
            inventoryObject.name +
            " moved from the hand slot to the inventory slot."
        );

        RefreshUI();
        return true;
    }

    public bool TryTakeInventoryItem()
    {
        if (pickupobject.heldObject != null)
        {
            Debug.Log("The hand slot is already occupied.");
            return false;
        }

        if (inventoryObject == null)
        {
            Debug.Log("Inventory slot is empty.");
            return false;
        }

        GameObject item = inventoryObject;
        pickupobject itemPickup = item.GetComponent<pickupobject>();

        if (itemPickup == null)
        {
            Debug.LogWarning(
                item.name +
                " cannot be returned because it has no pickupobject script."
            );

            inventoryObject = null;
            RefreshUI();
            return false;
        }

        item.SetActive(true);
        itemPickup.InitiatePickup();

        inventoryObject = null;

        Debug.Log(
            item.name +
            " moved from the inventory slot back to the hand."
        );

        RefreshUI();
        return true;
    }

    private void RefreshUI()
    {
        RefreshHandSlot();
        RefreshInventorySlot();
    }

    /// <summary>
    /// Formerly HandSlotSystem: shows whatever the original pickup system
    /// is currently holding.
    /// </summary>
    private void RefreshHandSlot()
    {
        if (handSlot == null)
        {
            return;
        }

        handSlot.SetSprite(
            GetSpriteFromObject(pickupobject.heldObject)
        );
    }

    private void RefreshInventorySlot()
    {
        if (inventorySlot == null)
        {
            return;
        }

        inventorySlot.SetSprite(
            GetSpriteFromObject(inventoryObject)
        );
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