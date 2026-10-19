using UnityEngine;
using UnityEngine.InputSystem;

public class AssemblyInventorySystem : MonoBehaviour
{
    [Header("Assembly UI")]
    [SerializeField] private GameObject assemblyUIPanel;

    [Header("Assembly Slots - Assign Four Different Slots")]
    [SerializeField] private InventorySlotUI[] assemblySlots = new InventorySlotUI[4];

    [Header("Allowed Object Tags")]
    [SerializeField] private string[] allowedTags = { "Item", "Equipment" };

    [Header("Settings")]
    [SerializeField] private bool assemblyStartsVisible = false;

    private readonly GameObject[] storedItems = new GameObject[4];
    private pickupobject pickup;

    private void Awake()
    {
        pickup = GetComponent<pickupobject>();
        if (pickup == null)
            Debug.LogError("AssemblyInventorySystem requires pickupobject on the same GameObject.", this);
    }

    private void Start()
    {
        if (assemblySlots == null || assemblySlots.Length != 4)
        {
            Debug.LogError("AssemblyInventorySystem requires exactly four slots.", this);
            return;
        }

        for (int i = 0; i < assemblySlots.Length; i++)
        {
            if (assemblySlots[i] == null)
                Debug.LogError("Assembly slot " + (i + 1) + " is not assigned.", this);

            for (int j = i + 1; j < assemblySlots.Length; j++)
            {
                if (assemblySlots[i] != null && assemblySlots[i] == assemblySlots[j])
                    Debug.LogError("Assembly slots " + (i + 1) + " and " + (j + 1) +
                                   " reference the same UI object. Assign four different slot objects.", this);
            }
        }

        if (assemblyUIPanel != null)
            assemblyUIPanel.SetActive(assemblyStartsVisible);
        else
            Debug.LogError("Assign KoboldAssemblystations to Assembly UI Panel.", this);

        RefreshSlots();
    }

    private void Update()
    {
        if (assemblyUIPanel == null || !assemblyUIPanel.activeInHierarchy ||
            assemblySlots == null || Mouse.current == null ||
            !Mouse.current.leftButton.wasPressedThisFrame)
            return;

        for (int i = 0; i < assemblySlots.Length; i++)
        {
            InventorySlotUI slot = assemblySlots[i];
            if (slot != null && IsMouseOverSlot(slot))
            {
                HandleSlotClick(i);
                return;
            }
        }
    }

    private bool IsMouseOverSlot(InventorySlotUI slot)
    {
        RectTransform rect = slot.GetComponent<RectTransform>();
        if (rect == null)
            return false;

        Canvas canvas = rect.GetComponentInParent<Canvas>();
        Camera uiCamera = null;
        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            uiCamera = canvas.worldCamera;

        return RectTransformUtility.RectangleContainsScreenPoint(
            rect, Mouse.current.position.ReadValue(), uiCamera);
    }

    private void HandleSlotClick(int slotIndex)
    {
        if (pickup == null)
            return;

        if (storedItems[slotIndex] != null)
        {
            if (pickupobject.heldObject == null)
                TakeItemFromSlot(slotIndex);
            return;
        }

        GameObject heldItem = pickupobject.heldObject;
        if (heldItem != null)
            StoreHeldItemInSlot(slotIndex, heldItem);
    }

    private bool HasAllowedTag(GameObject item)
    {
        if (item == null || allowedTags == null)
            return false;

        foreach (string allowedTag in allowedTags)
        {
            if (!string.IsNullOrEmpty(allowedTag) && item.tag == allowedTag)
                return true;
        }

        return false;
    }

    public bool StoreHeldItemInSlot(int slotIndex, GameObject item)
    {
        if (pickup == null || slotIndex < 0 || slotIndex >= storedItems.Length ||
            item == null || storedItems[slotIndex] != null)
            return false;

        if (!HasAllowedTag(item))
        {
            Debug.Log(item.name + " cannot be stored. Allowed tags: Item, Equipment.");
            return false;
        }

        if (pickupobject.heldObject != item)
            return false;

        // Clear state on the ITEM's pickupobject. heldObject is static,
        // so clearing the Kobold's component would not clear the item's held state.
        pickupobject itemPickup = item.GetComponent<pickupobject>();
        if (itemPickup == null)
        {
            Debug.LogWarning(item.name + " has no pickupobject component and cannot be stored.", item);
            return false;
        }

        storedItems[slotIndex] = item;
        itemPickup.ClearHeldState();
        item.SetActive(false);
        RefreshSlot(slotIndex);

        Debug.Log(item.name + " stored in assembly slot " + (slotIndex + 1) + ".");
        return true;
    }

    public bool TakeItemFromSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= storedItems.Length)
            return false;

        GameObject item = storedItems[slotIndex];
        if (item == null || pickup == null || pickupobject.heldObject != null)
            return false;

        pickupobject itemPickup = item.GetComponent<pickupobject>();
        if (itemPickup == null)
        {
            Debug.LogWarning(item.name + " has no pickupobject component and cannot be returned to the hand.", item);
            return false;
        }

        Transform playerTransform = pickup.GetPlayerTransform;
        if (playerTransform == null)
            playerTransform = pickupobject.FindPlayerTransform();

        // Remove it from the slot before starting the normal pickup sequence.
        storedItems[slotIndex] = null;
        if (playerTransform != null)
        {
            item.transform.position = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y,
                item.transform.position.z);
        }

        item.SetActive(true);
        itemPickup.InitiatePickup();
        RefreshSlot(slotIndex);

        Debug.Log(item.name + " taken from assembly slot " + (slotIndex + 1) + " and returned to the hand.");
        return true;
    }

    public GameObject GetAssemblyItem(int slotIndex)
    {
        return slotIndex >= 0 && slotIndex < storedItems.Length ? storedItems[slotIndex] : null;
    }

    public bool HasAssemblyItem(int slotIndex) => GetAssemblyItem(slotIndex) != null;
    public bool IsSlotOccupied(int slotIndex) => HasAssemblyItem(slotIndex);

    private void RefreshSlots()
    {
        if (assemblySlots == null)
            return;

        for (int i = 0; i < assemblySlots.Length; i++)
            RefreshSlot(i);
    }

    private void RefreshSlot(int slotIndex)
    {
        if (assemblySlots == null || slotIndex < 0 || slotIndex >= assemblySlots.Length)
            return;

        InventorySlotUI slot = assemblySlots[slotIndex];
        if (slot == null)
            return;

        GameObject item = storedItems[slotIndex];
        SpriteRenderer spriteRenderer = item != null
            ? item.GetComponentInChildren<SpriteRenderer>(true)
            : null;

        slot.SetSprite(spriteRenderer != null ? spriteRenderer.sprite : null);
        slot.SetVisible(true);
    }

    public void SetAssemblyVisible(bool visible)
    {
        if (assemblyUIPanel != null)
            assemblyUIPanel.SetActive(visible);
    }
}
