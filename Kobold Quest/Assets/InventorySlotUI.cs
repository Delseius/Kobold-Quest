//using System.Diagnostics;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;


/// <summary>
/// Put this on the InventorySlot prefab root.
/// It only knows how to show or hide its icon.
/// </summary>
public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    private CanvasGroup group;

    public bool IsVisible => GetGroup().alpha > 0f;

    /// <summary>
    /// Shows or hides the slot WITHOUT deactivating the GameObject,
    /// so the hierarchy and any layout positions stay exactly as built.
    /// </summary>
    public void SetVisible(bool visible)
    {
        CanvasGroup g = GetGroup();
        g.alpha = visible ? 1f : 0f;
        g.blocksRaycasts = visible;
        g.interactable = visible;
    }

    private CanvasGroup GetGroup()
    {
        if (group == null)
        {
            group = GetComponent<CanvasGroup>();

            if (group == null)
            {
                group = gameObject.AddComponent<CanvasGroup>();
            }
        }

        return group;
    }

    private void Awake()
    {
        Image background = GetComponent<Image>();

        // If the icon was never assigned (or the slot's own background
        // was assigned by mistake), look for the child Image instead.
        if (iconImage == null || iconImage == background)
        {
            foreach (Image img in GetComponentsInChildren<Image>(true))
            {
                if (img != background)
                {
                    iconImage = img;
                    break;
                }
            }

            if (iconImage == null)
            {
                Debug.LogError(
                    name + ": no child Image found to use as the icon.",
                    this
                );
            }
            else
            {
                Debug.LogWarning(
                    name + ": Icon Image was not assigned correctly. " +
                    "Using child '" + iconImage.name + "' instead.",
                    this
                );
            }
        }
    }

    public void SetSprite(Sprite sprite)
    {
        if (iconImage == null)
        {
            return;
        }

        iconImage.sprite = sprite;
        iconImage.color = Color.white;     // make sure alpha isn't 0
        iconImage.preserveAspect = true;
        iconImage.enabled = sprite != null;
    }
}
