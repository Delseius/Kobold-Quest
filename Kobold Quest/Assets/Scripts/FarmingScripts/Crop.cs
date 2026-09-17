using UnityEngine;

public abstract class Crop : MonoBehaviour
{
    [Header("Growth")]
    [SerializeField]
    protected float growthTime = 60f;

    protected float currentGrowthTime;

    [Header("Harvest")]
    [SerializeField]
    private bool survivesHarvest = false;

    [Header("Harvest Products")]
    [SerializeField]
    private CropHarvest[] harvestProducts;

    public bool IsMature { get; protected set; }

    public bool SurvivesHarvest => survivesHarvest;

    public CropHarvest[] HarvestProducts => harvestProducts;

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

    public virtual void ResetGrowth()
    {
        currentGrowthTime = 0f;
        IsMature = false;
    }
}