using UnityEngine;

[System.Serializable]
public class CropHarvest
{
    [SerializeField]
    private GameObject itemPrefab;

    [SerializeField]
    private int minQuantity = 1;

    [SerializeField]
    private int maxQuantity = 1;

    public GameObject ItemPrefab => itemPrefab;
    public int MinQuantity => minQuantity;
    public int MaxQuantity => maxQuantity;

    public int GetQuantity()
    {
        return Random.Range(minQuantity, maxQuantity + 1);
    }
}