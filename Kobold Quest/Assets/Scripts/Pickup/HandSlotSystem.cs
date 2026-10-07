using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays the object currently held by the original pickup system.
/// This is only a UI hand slot. It does not add an inventory slot and
/// does not modify pickupobject, PickupInput, PickupMovement, or
/// PickupDropSystem.
/// </summary>
public class HandSlotSystem : MonoBehaviour
{
    [Header("Hand Slot Appearance")]
    [SerializeField] private float slotSize = 72f;
    [SerializeField] private float iconSize = 54f;
    [SerializeField] private float bottomOffset = 28f;

    private Canvas canvas;
    private Image handIcon;
    private Sprite slotSprite;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateRuntimeHandSlot()
    {
        if (FindFirstObjectByType<HandSlotSystem>() != null)
            return;

        GameObject handSlotObject = new GameObject("HandSlotSystem");
        handSlotObject.AddComponent<HandSlotSystem>();
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        CreateUI();
        RefreshUI();
    }

    private void Update()
    {
        RefreshUI();
    }

    private void CreateUI()
    {
        GameObject canvasObject = new GameObject(
            "KoboldHandSlotCanvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );

        canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        slotSprite = CreateWhiteSprite();
        handIcon = CreateSlot();
    }

    private Image CreateSlot()
    {
        GameObject slotObject = new GameObject(
            "HandSlot",
            typeof(RectTransform),
            typeof(Image)
        );

        slotObject.transform.SetParent(canvas.transform, false);

        RectTransform slotRect = slotObject.GetComponent<RectTransform>();
        slotRect.anchorMin = new Vector2(0.5f, 0f);
        slotRect.anchorMax = new Vector2(0.5f, 0f);
        slotRect.pivot = new Vector2(0.5f, 0f);
        slotRect.anchoredPosition = new Vector2(0f, bottomOffset);
        slotRect.sizeDelta = new Vector2(slotSize, slotSize);

        Image slotImage = slotObject.GetComponent<Image>();
        slotImage.sprite = slotSprite;
        slotImage.color = new Color(0.08f, 0.08f, 0.08f, 0.82f);
        slotImage.raycastTarget = false;

        Outline outline = slotObject.AddComponent<Outline>();
        outline.effectColor = new Color(1f, 1f, 1f, 0.7f);
        outline.effectDistance = new Vector2(2f, 2f);

        GameObject iconObject = new GameObject(
            "HandIcon",
            typeof(RectTransform),
            typeof(Image)
        );

        iconObject.transform.SetParent(slotObject.transform, false);

        RectTransform iconRect = iconObject.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.5f, 0.5f);
        iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.anchoredPosition = Vector2.zero;
        iconRect.sizeDelta = new Vector2(iconSize, iconSize);

        Image icon = iconObject.GetComponent<Image>();
        icon.preserveAspect = true;
        icon.raycastTarget = false;

        return icon;
    }

    private void RefreshUI()
    {
        if (handIcon == null)
            return;

        GameObject heldObject = pickupobject.heldObject;

        Sprite sprite = GetSpriteFromObject(heldObject);

        handIcon.sprite = sprite;
        handIcon.enabled = sprite != null;
    }

    private Sprite GetSpriteFromObject(GameObject objectToRead)
    {
        if (objectToRead == null)
            return null;

        SpriteRenderer renderer =
            objectToRead.GetComponent<SpriteRenderer>();

        if (renderer != null)
            return renderer.sprite;

        SpriteRenderer childRenderer =
            objectToRead.GetComponentInChildren<SpriteRenderer>();

        if (childRenderer != null)
            return childRenderer.sprite;

        return null;
    }

    private Sprite CreateWhiteSprite()
    {
        Texture2D texture = new Texture2D(
            1,
            1,
            TextureFormat.RGBA32,
            false
        );

        texture.name = "HandSlotTexture";
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
