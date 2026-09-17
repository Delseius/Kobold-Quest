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




    bool Pressed = false;
    bool release = false;
    public static GameObject heldObject;
    //private bool Holding = false;
    private Rigidbody2D ObjectBody;
    private Collider2D ObjectCollider;
    //[SerializeField] private Vector3 PosOffset = new Vector3(0, 0, 0);
    //SceneManager.LoadScene(0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void start()
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
            UnityEngine.Debug.Log("pickup");
            if (ObjectBody != null)
            {
                ObjectBody.bodyType = RigidbodyType2D.Dynamic;
            }
            if (ObjectCollider != null)
            {
                ObjectCollider.enabled = false;
            }
            Vector3 ObjectOffset = new Vector3(88, 0, 0);
            transform.position = PlayerTransform.position += ObjectOffset;
            ObjectBody.bodyType = RigidbodyType2D.Kinematic;
            
            

            heldObject = gameObject;
            release = true;
            Pressed = false;
            
        }
        else if (Pressed && gameObject.CompareTag("Block") && heldObject != null)
        {
            UnityEngine.Debug.Log("drop");
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

                // Clear the reference so you can pick up a tool again later
                heldObject = null;
            }
            else
            {
                UnityEngine.Debug.LogWarning("Cannot teleport! No object is currently held.");
            }

            release = false;
            Pressed = false;
        }
        else if (Pressed && gameObject.CompareTag("Block"))
        {
            UnityEngine.Debug.Log("pickup2");
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
            transform.position = PlayerTransform.position;
            heldObject = gameObject;
            release = true;
            Pressed = false;

        }
        else if(1==1){
            transform.position = PlayerTransform.position;
        }
        
        /*if (Holding && PlayerTransform != null)
        {
            transform.position = PlayerTransform.position + PosOffset;
        }*/
        transform.position = PlayerTransform.position;

    }
}
