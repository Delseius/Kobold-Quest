using UnityEngine;

public class ConsumeFood : MonoBehaviour
{
    public float calories = 25f;

    public void Consume(GameObject consumer)
    {
        KoboldHunger hunger = consumer.GetComponent<KoboldHunger>();

        if (hunger != null)
        {
            hunger.ConsumeCalories(calories);
            Destroy(gameObject);
        }
    }
}
