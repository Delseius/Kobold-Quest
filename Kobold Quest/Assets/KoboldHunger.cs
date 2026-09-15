using UnityEngine;

public class KoboldHunger : MonoBehaviour
{
    public float maxCalories = 1600f;
    public float currentCalories = 1600f;

    public void ConsumeCalories(float calories)
    {
        currentCalories += calories;

        if (currentCalories > maxCalories)
        {
            currentCalories = maxCalories;
        }
    }
}