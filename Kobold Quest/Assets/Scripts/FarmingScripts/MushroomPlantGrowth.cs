using UnityEngine;

// Attach to the growing mushroom prefab, not the held item or food prefab.
public sealed class MushroomPlantGrowth : Crop
{
    private bool planted;

    public void BeginGrowing()
    {
        growthTime = 10f;
        ResetGrowth();
        planted = true;
    }

    protected override void Update()
    {
        if (planted)
        {
            base.Update();
        }
    }
}
