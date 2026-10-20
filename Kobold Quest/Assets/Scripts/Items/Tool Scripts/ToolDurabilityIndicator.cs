
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

    private Tool tool;
    private SpriteRenderer spriteRenderer;

    private Canvas barCanvas;
    private Image backgroundImage;
    private Image fillImage;
    private RectTransform fillRectTransform;

    private float hideTime;
    private bool isVisible;
    private int observedMaxDurability;

    private void Awake()
    {
        tool = GetComponent<Tool>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (tool == null)
        {
            Debug.LogWarning(
                "ToolDurabilityIndicator requires a Tool component.",
                this
            );

            enabled = false;
            return;
        }

        observedMaxDurability = Mathf.Max(
            maxDurability,
            tool.toolDurability
        );

        CreateBar();
        UpdateBar();
        SetVisible(false);
    }

    private void Update()
    {
        if (tool == null)
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
        if (tool == null)
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

        if (spriteRenderer != null)
        {
            barCanvas.sortingLayerID = spriteRenderer.sortingLayerID;
            barCanvas.sortingOrder = spriteRenderer.sortingOrder + 10;
        }
        else
        {
            barCanvas.sortingOrder = 10;
        }

        RectTransform canvasRect =
            canvasObject.GetComponent<RectTransform>();

        canvasRect.sizeDelta = new Vector2(
            barSize.x * 100f,
            barSize.y * 100f
        );

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

        fillRectTransform =
            fillObject.GetComponent<RectTransform>();

        fillRectTransform.anchorMin = Vector2.zero;
        fillRectTransform.anchorMax = Vector2.one;
        fillRectTransform.offsetMin = new Vector2(1f, 1f);
        fillRectTransform.offsetMax = new Vector2(-1f, -1f);

        fillImage = fillObject.GetComponent<Image>();
        fillImage.color = fillColor;
        fillImage.type = Image.Type.Simple;
        fillImage.raycastTarget = false;
    }

    private void UpdateBar()
    {
        if (fillImage == null || tool == null)
        {
            return;
        }

        if (tool.toolDurability > observedMaxDurability)
        {
            observedMaxDurability = tool.toolDurability;
        }

        float durabilityRatio = observedMaxDurability > 0
            ? (float)tool.toolDurability / observedMaxDurability
            : 0f;

        durabilityRatio = Mathf.Clamp01(durabilityRatio);

        // Change the width of the fill directly.
        if (fillRectTransform != null)
        {
            fillRectTransform.anchorMax = new Vector2(
                durabilityRatio,
                1f
            );

            fillRectTransform.offsetMax = new Vector2(-1f, -1f);
        }
    }

    private void LateUpdate()
    {
        if (spriteRenderer == null || barCanvas == null)
        {
            return;
        }

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

    private void SetVisible(bool visible)
    {
        isVisible = visible;

        if (barCanvas != null)
        {
            barCanvas.gameObject.SetActive(visible);
        }
    }
}
