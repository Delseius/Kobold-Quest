using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Separate four-slot storage for the assembly UI.
/// Assign the four AssemblySlotUI components in the Inspector.
/// The assembly slots accept objects with any tag listed in allowedTags.
/// InventorySystem remains separate and filters its own slots to Equipment.
/// </summary>
public class AssemblyInventorySystem : MonoBehaviour
{
    [Header("Assembly UI")]
    [SerializeField] private GameObject assemblyUIPanel;
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
    }

    private void Start()
    {
        if (assemblyUIPanel != null)
            assemblyUIPanel.SetActive(assemblyStartsVisible);

        RefreshUI();
    }

    private void Update()
    {
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
    }

    public void SetAssemblyVisible(bool visible)
    {
        if (assemblyUIPanel != null)
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
