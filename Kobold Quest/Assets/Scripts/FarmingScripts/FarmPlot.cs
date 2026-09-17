using UnityEngine;

public class FarmPlot : MonoBehaviour
{
    [SerializeField]
    private FarmPlotState state = FarmPlotState.Empty;

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
}