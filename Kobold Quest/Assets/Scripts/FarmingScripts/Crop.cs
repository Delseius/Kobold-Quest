using UnityEngine;

public abstract class Crop : MonoBehaviour
{
    [SerializeField]
    protected float growthTime = 60f;

    protected float currentGrowthTime;

    public bool IsMature { get; protected set; }

    protected virtual void Update()
    {
        if (IsMature)
        {
            return;
        }

        currentGrowthTime += Time.deltaTime;

        if (currentGrowthTime >= growthTime)
        {
            GrowComplete();
        }
    }

    protected virtual void GrowComplete()
    {
        IsMature = true;
    }
}