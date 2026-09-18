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
        //KoboldHunger hunger =
            //consumer.GetComponent<KoboldHunger>();

        //if (hunger == null)
        //{
            //return;
       // }

        //hunger.ConsumeCalories(calories);

        Destroy(gameObject);
        Debug.Log("Food eaten by " + consumer.name + ". Calories consumed: " + calories);

    }

    public virtual bool Use(GameObject user)
    {
        Eat(user);

        return true;
    }
}