using UnityEngine;

public class KoboldThirst : MonoBehaviour
{
    public float maxHydration = 100f;
    public float currentHydration = 100f;

    public float hydrationLostPerSecond = 0.5f;

    private void Update()
    {
        LoseHydrationOverTime(Time.deltaTime);
    }


    public void Drink(float amount)
    {
        if (amount < 0f)
        {
            return;
        }

        currentHydration += amount;

        if (currentHydration > maxHydration)
        {
            currentHydration = maxHydration;
        }
    }

    public void LoseHydration(float amount)
    {
        if (amount < 0f)
        {
            return;
        }

        currentHydration -= amount;

        if (currentHydration < 0f)
        {
            currentHydration = 0f;
        }
    }


    public void LoseHydrationOverTime(float elapsedSeconds)
    {
        float hydrationToLose =
            hydrationLostPerSecond * elapsedSeconds;

        LoseHydration(hydrationToLose);
    }

    public float GetThirstPercentage()
    {
        if (maxHydration <= 0f)
        {
            return 0f;
        }

        return currentHydration / maxHydration;
    }
}
