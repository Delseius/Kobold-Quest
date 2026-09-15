using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class pickupobject : MonoBehaviour
{
    [SerializeField] private Transform PlayerTransform;
    [SerializeField] private Transform BlockTransform;
    [SerializeField] private Transform ToolTransform;
    [SerializeField] private Transform ConsumableTransform;
    [SerializeField] private Transform PlaceBlockTransform;
    [SerializeField] private Collider2D normalCollider;
    [SerializeField] private Collider2D temporaryCollider;




    bool Pressed = false;
    bool release = false;
    bool held = false;
    bool Tool = false;
    bool block = false;
    bool drop = false;
    //bool block = false;
    public float speed = 5.5F;
    public static GameObject heldObject;
    
    //private bool Holding = false;
    private Rigidbody2D ObjectBody;
    private Collider2D ObjectCollider;
    //[SerializeField] private Vector3 PosOffset = new Vector3(0, 0, 0);
    //SceneManager.LoadScene(0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ObjectBody = GetComponent<Rigidbody2D>();
        ObjectCollider = GetComponent<Collider2D>();
    }

    private void OnMouseDown()
    { 
        Pressed = true;
        Debug.Log($"You clicked directly on {gameObject.name}!");
        if (ObjectBody != null)
        {
            ObjectBody.bodyType = RigidbodyType2D.Kinematic;
        }
        //SceneManager.LoadScene(1);
    }

    void Update()
    {
        if (Pressed && gameObject.CompareTag("Tools") && !release)
        {
            UnityEngine.Debug.Log("pickuptool");
            if (ObjectBody != null)
            {
                ObjectBody.bodyType = RigidbodyType2D.Dynamic;
            }
            if (ObjectCollider != null)
            {
                ObjectCollider.enabled = false;
            }
            Vector3 ObjectOffset = new Vector3(7, 0, 0);
            ToolTransform.position = PlayerTransform.position;
            PlayerTransform.position = Vector3.MoveTowards(PlayerTransform.position, ToolTransform.position, speed * Time.deltaTime);
            ObjectBody.bodyType = RigidbodyType2D.Kinematic;



            heldObject = gameObject;
            release = true;
            Pressed = false;
            held = true;
            Tool = true;

        }
        
        else if (Pressed && gameObject.CompareTag("Block") && heldObject != null)
        {
            UnityEngine.Debug.Log("Drop");
            if (ObjectCollider != null)
            {
                ObjectCollider.enabled = true;
            }
            //Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            //transform.position = mousePos;
            if (heldObject != null)
            {
                // Teleports the held tool to this block's position setup
                heldObject.transform.position = BlockTransform.position;
                PlayerTransform.position = BlockTransform.position;

                // Clear the reference so you can pick up a tool again later
                heldObject = null;
            }
            else
            {
                UnityEngine.Debug.LogWarning("Cannot teleport! No object is currently held.");
            }

            release = false;
            Pressed = false;
            held = false;
            Tool = false;

        }
        else if (Pressed && gameObject.CompareTag("Block") && !release)
        {
            UnityEngine.Debug.Log("pickupBlock");
            //Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            //transform.position = mousePos;
            //GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            //ObjectCollider.enabled = false;
            if (ObjectBody != null)
            {
                ObjectBody.bodyType = RigidbodyType2D.Dynamic;
            }
            if (ObjectCollider != null)
            {
                ObjectCollider.enabled = false;
            }
            BlockTransform.position = PlayerTransform.position;
            heldObject = gameObject;
            release = true;
            Pressed = false;
            held = true;
            Tool = false;


        }
        if (held == true && Tool == true)
        {
            //UnityEngine.Debug.Log("held");
            // Smoothly move the object toward the player's position every frame
            Vector3 ObjectOffset = new Vector3(2.0f, 0.0f, 0.0f);
            ToolTransform.position = Vector3.MoveTowards(ToolTransform.position, PlayerTransform.position + ObjectOffset, speed * Time.deltaTime);
        }


        /*if (Holding && PlayerTransform != null)
        {
            transform.position = PlayerTransform.position + PosOffset;
        }*/
        //transform.position = PlayerTransform.position;

    }
}
