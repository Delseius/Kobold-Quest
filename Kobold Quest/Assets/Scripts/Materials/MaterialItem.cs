using UnityEngine;

/// <summary>
/// Describes the identity and physical properties of a material item.
/// Volume is measured in cubic metres (m^3), density in kg/m^3,
/// and calculated mass in kilograms (kg).
/// </summary>
public class MaterialItem : MonoBehaviour
{
    // Approximate air-dry density for radiata pine, used as a practical
    // real-world reference for the game's generic surface wood.
    public const float DefaultWoodDensityKgPerCubicMetre = 535f;

    // Representative density for common solid rock, approximated using granite.
    public const float DefaultStoneDensityKgPerCubicMetre = 2700f;

    // Approximate volume of a regulation-size soccer ball (about 22 cm diameter).
    public const float DefaultStoneVolumeCubicMetres = 0.0055f;

    [Header("Identity")]
    [SerializeField] private ItemID itemID;
    [SerializeField] private MaterialType materialType = MaterialType.SurfaceWood;

    [Header("Physical Properties")]
    [SerializeField, Min(0f)] private float volumeCubicMetres;
    [SerializeField, Min(0f)] private float densityKgPerCubicMetre =
        DefaultWoodDensityKgPerCubicMetre;

    public ItemID ItemID => itemID;
    public MaterialType MaterialType => materialType;

    /// <summary>Volume in cubic metres.</summary>
    public float VolumeCubicMetres
    {
        get => volumeCubicMetres;
        set => volumeCubicMetres = Mathf.Max(0f, value);
    }

    /// <summary>Density in kilograms per cubic metre.</summary>
    public float DensityKgPerCubicMetre
    {
        get => densityKgPerCubicMetre;
        set => densityKgPerCubicMetre = Mathf.Max(0f, value);
    }

    /// <summary>Calculated mass in kilograms. Mass is not stored separately.</summary>
    public float MassKg => VolumeCubicMetres * DensityKgPerCubicMetre;

    private void OnValidate()
    {
        volumeCubicMetres = Mathf.Max(0f, volumeCubicMetres);
        densityKgPerCubicMetre = Mathf.Max(0f, densityKgPerCubicMetre);
    }
}
