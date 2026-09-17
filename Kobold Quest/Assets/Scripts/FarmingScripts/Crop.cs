using UnityEngine;

public abstract class Crop : MonoBehaviour
{
    [Header("Growth")]
    [SerializeField]
    protected float growthTime = 60f;

    protected float currentGrowthTime;

    [Header("Harvest")]
    [SerializeField]
    private int minHarvestYield = 2;

    [SerializeField]
    private int maxHarvestYield = 3;

    [SerializeField]
    private bool survivesHarvest = false;

    public bool IsMature { get; protected set; }

    public int MinHarvestYield => minHarvestYield;
    public int MaxHarvestYield => maxHarvestYield;
    public bool SurvivesHarvest => survivesHarvest;

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

    public int GetHarvestYield()
    {
        return Random.Range(minHarvestYield, maxHarvestYield + 1);
    }
    public virtual void ResetGrowth()
    {
        currentGrowthTime = 0f;
        IsMature = false;
    }
}