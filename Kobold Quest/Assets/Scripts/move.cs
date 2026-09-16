
using UnityEngine;
using UnityEngine.InputSystem;

public class move : MonoBehaviour
{
    public float moveSpeed = 5f;
    

    
    private Rigidbody2D rb;
    private Vector2 moveXY;

    
    void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        moveXY = context.ReadValue<Vector2>();
    }


    void FixedUpdate()
    {

        rb.linearVelocity = moveXY * moveSpeed;
    }
}
