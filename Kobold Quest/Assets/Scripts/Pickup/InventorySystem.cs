using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// One-slot inventory matching the Kobold Quest pickup system.
///
/// The hand slot mirrors the object currently held by pickupobject.heldObject.
/// Press Q while holding something to move it into the inventory slot.
/// Press Q again with an empty hand to take the inventory item back into the hand.
///
/// The UI is created at runtime, so no Canvas or scene changes are required.
/// It stays at the bottom of the screen because it uses Screen Space Overlay.
/// </summary>
public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    [Header("Controls")]
    [SerializeField] private bool useQKey = true;

    [Header("Slot Appearance")]
    [SerializeField] private float slotSize = 72f;
    [SerializeField] private float iconSize = 54f;
    [SerializeField] private float slotSpacing = 12f;
    [SerializeField] private float bottomOffset = 28f;

    private GameObject inventoryObject;

    private Canvas canvas;
    private Image handIcon;
    private Image inventoryIcon;
    private Sprite slotSprite;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateRuntimeInventory()
    {
        if (FindFirstObjectByType<InventorySystem>() != null)
        {
            return;
        }

        GameObject inventoryObject = new GameObject("InventorySystem");
        inventoryObject.AddComponent<InventorySystem>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        CreateUI();
        RefreshUI();
    }

    private void Update()
    {
        if (useQKey &&
            Keyboard.current != null &&
            Keyboard.current.qKey.wasPressedThisFrame)
        {
            if (pickupobject.heldObject != null)
            {
                TryStoreHeldItem();
            }
            else
            {
                TryTakeInventoryItem();
            }
        }

        RefreshUI();
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

        inventoryObject = held;
        heldPickup.StoreInInventory();

        Debug.Log(
            held.name +
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
                " cannot be equipped because it has no pickupobject script."
            );
            inventoryObject = null;
            RefreshUI();
            return false;
        }

        inventoryObject = null;

        item.SetActive(true);
        itemPickup.EquipFromInventory();

        Debug.Log(
            item.name +
            " moved from the inventory slot to the hand slot."
        );

        RefreshUI();
        return true;
    }

    private void CreateUI()
    {
        GameObject canvasObject = new GameObject(
            "KoboldInventoryCanvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );

        canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler =
            canvasObject.GetComponent<CanvasScaler>();

        scaler.uiScaleMode =
            CanvasScaler.ScaleMode.ScaleWithScreenSize;

        scaler.referenceResolution =
            new Vector2(1920f, 1080f);

        slotSprite = CreateWhiteSprite();

        handIcon = CreateSlot(
            "HandSlot",
            new Vector2(
                -(slotSize + slotSpacing) * 0.5f,
                bottomOffset
            ),
            "HandIcon"
        );

        inventoryIcon = CreateSlot(
            "InventorySlot",
            new Vector2(
                (slotSize + slotSpacing) * 0.5f,
                bottomOffset
            ),
            "InventoryIcon"
        );
    }

    private Image CreateSlot(
        string slotName,
        Vector2 position,
        string iconName
    )
    {
        GameObject slotObject = new GameObject(
            slotName,
            typeof(RectTransform),
            typeof(Image)
        );

        slotObject.transform.SetParent(
            canvas.transform,
            false
        );

        RectTransform slotRect =
            slotObject.GetComponent<RectTransform>();

        slotRect.anchorMin =
            new Vector2(0.5f, 0f);

        slotRect.anchorMax =
            new Vector2(0.5f, 0f);

        slotRect.pivot =
            new Vector2(0.5f, 0f);

        slotRect.anchoredPosition = position;
        slotRect.sizeDelta =
            new Vector2(slotSize, slotSize);

        Image slotImage =
            slotObject.GetComponent<Image>();

        slotImage.sprite = slotSprite;
        slotImage.color =
            new Color(0.08f, 0.08f, 0.08f, 0.82f);

        slotImage.raycastTarget = false;

        Outline outline =
            slotObject.AddComponent<Outline>();

        outline.effectColor =
            new Color(1f, 1f, 1f, 0.7f);

        outline.effectDistance =
            new Vector2(2f, 2f);

        GameObject iconObject = new GameObject(
            iconName,
            typeof(RectTransform),
            typeof(Image)
        );

        iconObject.transform.SetParent(
            slotObject.transform,
            false
        );

        RectTransform iconRect =
            iconObject.GetComponent<RectTransform>();

        iconRect.anchorMin =
            new Vector2(0.5f, 0.5f);

        iconRect.anchorMax =
            new Vector2(0.5f, 0.5f);

        iconRect.pivot =
            new Vector2(0.5f, 0.5f);

        iconRect.anchoredPosition =
            Vector2.zero;

        iconRect.sizeDelta =
            new Vector2(iconSize, iconSize);

        Image icon =
            iconObject.GetComponent<Image>();

        icon.preserveAspect = true;
        icon.raycastTarget = false;

        return icon;
    }

    private void RefreshUI()
    {
        if (handIcon == null || inventoryIcon == null)
        {
            return;
        }

        GameObject held =
            pickupobject.heldObject;

        SetIcon(
            handIcon,
            GetSpriteFromObject(held)
        );

        SetIcon(
            inventoryIcon,
            GetSpriteFromObject(inventoryObject)
        );
    }

    private Sprite GetSpriteFromObject(GameObject objectToRead)
    {
        if (objectToRead == null)
        {
            return null;
        }

        SpriteRenderer renderer =
            objectToRead.GetComponent<SpriteRenderer>();

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

    private void SetIcon(Image icon, Sprite sprite)
    {
        icon.sprite = sprite;
        icon.enabled = sprite != null;
    }

    private Sprite CreateWhiteSprite()
    {
        Texture2D texture =
            new Texture2D(
                1,
                1,
                TextureFormat.RGBA32,
                false
            );

        texture.name = "InventorySlotTexture";
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0f, 0f, 1f, 1f),
            new Vector2(0.5f, 0.5f),
            1f
        );
    }
}
