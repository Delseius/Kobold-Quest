using UnityEngine;
using UnityEngine.InputSystem;

public class pickupobject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnMouseDown()
    {
        Debug.Log($"You clicked directly on {gameObject.name}!");
    }
}
