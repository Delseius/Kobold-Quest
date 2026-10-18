using UnityEngine;

public class KoboldHunger : MonoBehaviour
{
    public float maxCalories = 1600f;
    public float currentCalories = 1600f;

    public float caloriesLostPerSecond = 10f;

    private void Update()
    {
        LoseCaloriesOverTime(Time.deltaTime);
    }


    public void ConsumeCalories(float calories)
    {
        if (calories < 0f)
        {
            return;
        }

        currentCalories += calories;

        if (currentCalories > maxCalories)
        {
            currentCalories = maxCalories;
        }
    }

    public void LoseCalories(float calories)
    {
        if (calories < 0f)
        {
            return;
        }

        currentCalories -= calories;

        if (currentCalories < 0f)
        {
            currentCalories = 0f;
        }
    }


    public void LoseCaloriesOverTime(float elapsedSeconds)
    {
        float caloriesToLose =
            caloriesLostPerSecond * elapsedSeconds;

        LoseCalories(caloriesToLose);
    }

    public float GetHungerPercentage()
    {
        if (maxCalories <= 0f)
        {
            return 0f;
        }

        return currentCalories / maxCalories;
    }
}
