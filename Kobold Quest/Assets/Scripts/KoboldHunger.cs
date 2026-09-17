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

    public void LoseCalories(float calories)
    {
        currentCalories -= calories;

        if (currentCalories < 0f)
        {
            currentCalories = 0f;
        }
    }

    public float GetHungerPercentage()
    {
        return currentCalories / maxCalories;
    }
}