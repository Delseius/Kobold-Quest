using UnityEngine;

public abstract class Food : MonoBehaviour
{
    [SerializeField]
    protected float calories;

    public float Calories
    {
        get { return calories; }
    }

    public virtual void Eat(GameObject consumer)
    {
        KoboldHunger hunger = consumer.GetComponent<KoboldHunger>();

        if (hunger == null)
        {
            return;
        }

        hunger.ConsumeCalories(calories);

        Destroy(gameObject);
    }
}