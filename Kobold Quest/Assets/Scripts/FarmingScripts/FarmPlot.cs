using UnityEngine;

public class FarmPlot : MonoBehaviour
{
    [SerializeField]
    private FarmPlotState state = FarmPlotState.Empty;

    [SerializeField]
    private Crop cropPrefab;

    private Crop crop;

    public FarmPlotState State => state;
    public Crop CurrentCrop => crop;

    public bool CanPlant()
    {
        return state == FarmPlotState.Empty;
    }

    public bool CanHarvest()
    {
        return state == FarmPlotState.Mature;
    }

    public bool Plant()
    {
        if (!CanPlant())
        {
            return false;
        }

        if (cropPrefab == null)
        {
            Debug.LogError("No crop prefab assigned to FarmPlot.");
            return false;
        }

        crop = Instantiate(
            cropPrefab,
            transform.position,
            Quaternion.identity,
            transform
        );

        state = FarmPlotState.Growing;

        return true;
    }

    public Crop Harvest()
    {
        if (!CanHarvest())
        {
            return null;
        }

        Crop harvestedCrop = crop;

        crop = null;
        state = FarmPlotState.Empty;

        return harvestedCrop;
    }

    private void Update()
    {
        if (state == FarmPlotState.Growing && crop != null)
        {
            if (crop.IsMature)
            {
                state = FarmPlotState.Mature;
            }
        }
    }
}