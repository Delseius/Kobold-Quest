using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class move : MonoBehaviour
{
    public float moveSpeed = 5f;
    

    
    private Rigidbody2D rb;
    private float moveX;
    private float moveY;

    
    void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
        

    }

   
    void FixedUpdate()
    {
        
        rb.linearVelocity = new Vector2(moveX * moveSpeed, moveY * moveSpeed);
    }
}
