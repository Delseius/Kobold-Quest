
using UnityEngine;
using UnityEngine.UI;

public class ToolDurabilityIndicator : MonoBehaviour
{
    [Header("Durability Indicator")]
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private int maxDurability = 100;

    [Header("Bar Appearance")]
    [SerializeField] private Vector2 barSize = new Vector2(1f, 0.12f);
    [SerializeField] private float verticalOffset = 0.15f;
    [SerializeField] private Color backgroundColor = Color.gray;
    [SerializeField] private Color fillColor = Color.green;

    private Pickaxe pickaxe;
    private SpriteRenderer spriteRenderer;

    private Canvas barCanvas;
    private Image backgroundImage;
    private Image fillImage;

    private float hideTime;
    private bool isVisible;

    private int observedMaxDurability;
    private int previousDurability = int.MinValue;
    private RectTransform fillRectTransform;
    private float fullFillWidth;

    private void Awake()
    {
        pickaxe = GetComponent<Pickaxe>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (pickaxe == null)
        {
            Debug.LogWarning(
                "ToolDurabilityIndicator requires a Pickaxe component.",
                this
            );

            enabled = false;
            return;
        }

        CreateBar();
        UpdateBar();
        SetVisible(false);
    }

    private void Update()
    {
        if (pickaxe == null)
        {
            return;
        }

        UpdateBar();

        if (isVisible && Time.time >= hideTime)
        {
            SetVisible(false);
        }
    }

    public void ShowIndicator()
    {
        if (pickaxe == null)
        {
            return;
        }

        UpdateBar();

        hideTime = Time.time + displayDuration;
        SetVisible(true);
    }

    private void CreateBar()
    {
        GameObject canvasObject = new GameObject(
            "ToolDurabilityBar",
            typeof(RectTransform),
            typeof(Canvas)
        );

        canvasObject.transform.SetParent(transform, false);

        barCanvas = canvasObject.GetComponent<Canvas>();
        barCanvas.renderMode = RenderMode.WorldSpace;
        barCanvas.overrideSorting = true;
        barCanvas.sortingLayerID = spriteRenderer != null
            ? spriteRenderer.sortingLayerID
            : 0;

        barCanvas.sortingOrder = spriteRenderer != null
            ? spriteRenderer.sortingOrder + 10
            : 10;

        if (spriteRenderer != null)
        {
            barCanvas.sortingLayerID = spriteRenderer.sortingLayerID;
            barCanvas.sortingOrder = spriteRenderer.sortingOrder + 1;
        }

        barCanvas.sortingOrder = spriteRenderer != null
            ? spriteRenderer.sortingOrder + 10
            : 10;

        RectTransform canvasRect =
            canvasObject.GetComponent<RectTransform>();

        canvasRect.sizeDelta = new Vector2(100f, 12f);
        canvasRect.sizeDelta = new Vector2(
            barSize.x * 100f,
            barSize.y * 100f );

        canvasRect.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        float spriteHeight = spriteRenderer != null
            ? spriteRenderer.bounds.extents.y /
              Mathf.Max(Mathf.Abs(transform.lossyScale.y), 0.0001f)
            : 0.5f;

        canvasRect.localPosition = new Vector3(
            0f,
            spriteHeight + verticalOffset,
            0f
        );

        GameObject backgroundObject = new GameObject(
            "Background",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image)
        );

        backgroundObject.transform.SetParent(canvasObject.transform, false);

        RectTransform backgroundRect =
            backgroundObject.GetComponent<RectTransform>();

        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;

        backgroundImage = backgroundObject.GetComponent<Image>();
        backgroundImage.color = backgroundColor;
        backgroundImage.raycastTarget = false;

        GameObject fillObject = new GameObject(
            "Fill",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image)
        );

        fillObject.transform.SetParent(backgroundObject.transform, false);

        RectTransform fillRect =
            fillObject.GetComponent<RectTransform>();

        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(1f, 1f);
        fillRect.offsetMax = new Vector2(-1f, -1f);

        fillImage = fillObject.GetComponent<Image>();
        fillRectTransform = fillObject.GetComponent<RectTransform>();
        fullFillWidth = fillRectTransform.rect.width;
        fillImage.color = fillColor;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        fillImage.fillAmount = 1f;
        fillImage.raycastTarget = false;
    }

    private void UpdateBar()
    {
        if (fillImage == null || pickaxe == null)
        {
            return;
        }

        float durabilityRatio = maxDurability > 0
            ? (float)pickaxe.toolDurability / maxDurability
            : 0f;

        fillImage.fillAmount = Mathf.Clamp01(durabilityRatio);
    }

    private void SetVisible(bool visible)
    {
        isVisible = visible;

        if (barCanvas != null)
        {
            barCanvas.gameObject.SetActive(visible);
        }
    }

    private void LateUpdate()
    {
        if (pickaxe == null || fillImage == null)
        {
            return;
        }

        if (pickaxe.toolDurability != previousDurability)
        {
            Debug.Log(
                "Durability indicator detected change: " +
                previousDurability + " -> " +
                pickaxe.toolDurability,
                this );

            previousDurability = pickaxe.toolDurability;
        }

        // Track the highest durability value this tool has reached.
        if (pickaxe.toolDurability > observedMaxDurability)
        {
            observedMaxDurability = pickaxe.toolDurability;
            }

        // Update the green bar using the tool's actual durability.
        if (observedMaxDurability > 0)
        {
            fillImage.fillAmount = Mathf.Clamp01(
                (float)pickaxe.toolDurability / observedMaxDurability
            );
        }

        // Position the bar above the actual sprite bounds.
        if (spriteRenderer != null && barCanvas != null)
        {
            RectTransform canvasRect =
                barCanvas.GetComponent<RectTransform>();

            Vector3 worldPosition = new Vector3(
                spriteRenderer.bounds.center.x,
                spriteRenderer.bounds.max.y + verticalOffset,
                spriteRenderer.bounds.center.z
            );

            canvasRect.localPosition =
                transform.InverseTransformPoint(worldPosition);
        }
        if (fillRectTransform != null && observedMaxDurability > 0)
        {
            float ratio = Mathf.Clamp01(
                (float)pickaxe.toolDurability / observedMaxDurability);

            fillRectTransform.anchorMax = new Vector2(ratio, 1f);
            fillRectTransform.offsetMax = new Vector2(-1f, -1f);
        }
        
    }


}
