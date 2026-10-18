using UnityEngine;

public abstract class Food : MonoBehaviour, IUsable
{
    [SerializeField]
    protected float calories;

    public float Calories
    {
        get { return calories; }
    }


    public virtual void Eat(GameObject consumer)
    {
        if (consumer == null)
        {
            Debug.LogWarning("Cannot eat food without a consumer.");
            return;
        }

        KoboldHunger hunger =
            consumer.GetComponent<KoboldHunger>();

        if (hunger == null)
        {
            Debug.LogWarning(
                consumer.name + " cannot eat: no KoboldHunger component found."
            );
            return;
        }

        hunger.ConsumeCalories(calories);

        Debug.Log(
            "Food eaten by " + consumer.name +
            ". Calories restored: " + calories
        );

        Destroy(gameObject);
    }


    public virtual bool Use(GameObject user)
    {
        Eat(user);

        return true;
    }
}