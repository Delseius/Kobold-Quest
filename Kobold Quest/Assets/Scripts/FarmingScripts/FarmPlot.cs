using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class FarmPlot : MonoBehaviour
{
    [SerializeField] private FarmPlotState state = FarmPlotState.Empty;
    [SerializeField] private Crop cropPrefab;

    [Header("Mushroom Food")]
    [SerializeField] private GameObject mushroomFoodPrefab;
    [SerializeField] private Vector3 foodSpawnOffset = Vector3.zero;

    private Crop crop;

    public FarmPlotState State => state;
    public Crop CurrentCrop => crop;

    public bool CanPlant()
    {
        return state == FarmPlotState.Empty && crop == null;
    }

    public bool CanHarvest()
    {
        return state == FarmPlotState.Mature && crop != null;
    }

    public bool Plant()
    {
        if (!CanPlant()) return false;

        if (cropPrefab == null)
        {
            Debug.LogError("No crop prefab assigned to FarmPlot.", this);
            return false;
        }

        if (cropPrefab is MushroomPlantGrowth && mushroomFoodPrefab == null)
        {
            Debug.LogError("Assign Mushroom Food Prefab before planting.", this);
            return false;
        }

        crop = Instantiate(cropPrefab, transform.position, Quaternion.identity, transform);
        if (crop is MushroomPlantGrowth mushroom)
        {
            mushroom.BeginGrowing();
        }
        else
        {
            crop.ResetGrowth();
        }

        state = FarmPlotState.Growing;
        return true;
    }

    // Called only for the plot immediately in front of the player.
    public bool TryPlantHeldMushroom()
    {
        Debug.Log("Z detected by farm plot: " + name);

        if (!CanPlant())
        {
            Debug.Log("Plot is not empty. Current state: " + state);
            return false;
        }

        if (!IsInFrontOfPlayer())
        {
            Debug.Log("This plot is not in the grid cell directly in front of the player.");
            return false;
        }

        GameObject heldItem = pickupobject.heldObject;

        if (heldItem == null)
        {
            Debug.Log("pickupobject.heldObject is null. No held item detected.");
            return false;
        }

        Debug.Log("Held item: " + heldItem.name);

        if (!heldItem.activeInHierarchy)
        {
            Debug.Log("Held item is inactive.");
            return false;
        }

        if (heldItem.GetComponent<MushroomPlantingItem>() == null)
        {
            Debug.Log(
                "The held object needs MushroomPlantingItem on the same " +
                "GameObject referenced by pickupobject.heldObject."
            );
            return false;
        }

        if (!(cropPrefab is MushroomPlantGrowth))
        {
            Debug.LogError(
                "Crop Prefab must reference a prefab with MushroomPlantGrowth.",
                this
            );
            return false;
        }

        if (!Plant())
        {
            Debug.LogError("Plant() failed. Check the preceding Console error.", this);
            return false;
        }

        pickupobject pickup = heldItem.GetComponent<pickupobject>();

        if (pickup != null)
        {
            pickup.ClearHeldState();
        }

        pickupobject.heldObject = null;
        heldItem.SetActive(false);
        Destroy(heldItem);

        Debug.Log("Mushroom planted. Food will spawn after 10 seconds.");
        return true;
    }

    private bool IsInFrontOfPlayer()
    {
        GridSpace grid = GridSpace.Instance;
        if (grid == null) return false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        Vector2Int playerPosition = grid.WorldToGrid(player.transform.position);
        Vector2Int direction = player.transform.localScale.x < 0f
            ? Vector2Int.left : Vector2Int.right;
        Vector2Int targetPosition = playerPosition + direction;

        return grid.TryGetCell(targetPosition, out GridCell targetCell)
            && targetCell.Position == grid.WorldToGrid(transform.position);
    }

    public void Harvest()
    {
        if (!CanHarvest()) return;

        if (crop is MushroomPlantGrowth)
        {
            if (mushroomFoodPrefab == null) return;

            // No parent: the food keeps its prefab scale and is an independent pickup.
            Instantiate(mushroomFoodPrefab,
                transform.position + foodSpawnOffset, Quaternion.identity);

            ClearCrop();
            Debug.Log("Mushroom finished growing. Food spawned.", this);
            return;
        }

        // Preserve the existing harvest behaviour for other crop types.
        if (crop.HarvestProducts != null)
        {
            foreach (CropHarvest harvest in crop.HarvestProducts)
            {
                if (harvest == null || harvest.ItemPrefab == null) continue;
                Debug.Log("Harvested " + harvest.GetQuantity()
                    + " of " + harvest.ItemPrefab.name);
            }
        }

        if (crop.SurvivesHarvest)
        {
            crop.ResetGrowth();
            state = FarmPlotState.Growing;
        }
        else
        {
            ClearCrop();
        }
    }

    private void ClearCrop()
    {
        crop.gameObject.SetActive(false);
        Destroy(crop.gameObject);
        crop = null;
        state = FarmPlotState.Empty;
    }

    private static bool PlantKeyPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.zKey.wasPressedThisFrame;
#elif ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(KeyCode.Z);
#else
        return false;
#endif
    }

    private void Update()
    {
        if (PlantKeyPressed()) TryPlantHeldMushroom();

        if (state == FarmPlotState.Growing && crop != null && crop.IsMature)
        {
            state = FarmPlotState.Mature;
            if (crop is MushroomPlantGrowth) Harvest();
        }
    }
}
