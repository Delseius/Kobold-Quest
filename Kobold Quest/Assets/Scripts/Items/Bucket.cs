using UnityEngine;

// A bucket the kobold can hold. Press F (use) while holding it:
// - next to a WaterPool and not full -> fills the bucket
// - anywhere else with water in it    -> drinks one sip
public class Bucket : MonoBehaviour, IUsable
{
    [Header("Bucket")]
    [SerializeField]
    private int maxSips = 5;

    [SerializeField]
    private int currentSips = 0;

    [SerializeField]
    [Tooltip("Hydration restored per sip.")]
    private float hydrationPerSip = 20f;

    [Header("Sprites (optional)")]
    [SerializeField]
    private Sprite emptySprite;

    [SerializeField]
    private Sprite filledSprite;

    private SpriteRenderer spriteRenderer;

    public int CurrentSips
    {
        get { return currentSips; }
    }

    public bool IsEmpty
    {
        get { return currentSips <= 0; }
    }

    public bool IsFull
    {
        get { return currentSips >= maxSips; }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    public bool Use(GameObject user)
    {
        if (user == null)
        {
            return false;
        }

        if (!IsFull)
        {
            WaterPool pool =
                WaterPool.FindPoolInRange(user.transform.position);

            if (pool != null)
            {
                Fill(pool);
                return false;
            }
        }

        Drink(user);

        // The bucket is never used up, so keep holding it.
        return false;
    }

    public void Fill(WaterPool pool)
    {
        currentSips = maxSips;
        UpdateSprite();

        Debug.Log(
            $"Filled {name} at {pool.name}. " +
            $"Water: {currentSips}/{maxSips}"
        );
    }

    public bool Drink(GameObject drinker)
    {
        if (IsEmpty)
        {
            Debug.Log($"{name} is empty. Fill it at a water pool.");
            return false;
        }

        KoboldThirst thirst =
            drinker.GetComponent<KoboldThirst>();

        if (thirst == null)
        {
            Debug.LogWarning(
                drinker.name + " cannot drink: no KoboldThirst component found."
            );
            return false;
        }

        thirst.Drink(hydrationPerSip);
        currentSips--;
        UpdateSprite();

        Debug.Log(
            $"{drinker.name} drank from {name}. " +
            $"Hydration restored: {hydrationPerSip}. " +
            $"Water left: {currentSips}/{maxSips}"
        );

        return true;
    }

    private void UpdateSprite()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        Sprite sprite =
            IsEmpty ? emptySprite : filledSprite;

        if (sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
    }
}
