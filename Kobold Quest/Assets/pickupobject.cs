using System.Collections.Specialized;
//using System.Diagnostics;
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

    // REMOVED [SerializeField] from PlaceBlockTransform so it updates dynamically based on what you click!
    private Transform PlaceBlockTransform;

    [SerializeField] private Collider2D normalCollider;
    [SerializeField] private Collider2D temporaryCollider;
    [SerializeField] private float holdOffsetDistance = 1.2f;

    bool Pressed = false;
    bool release = false;
    bool held = false;
    bool Tool = false;
    bool block = false;
    bool drop = false;
    bool PlayerMove = false;

    public float speed = 5.5F;
    public static GameObject heldObject;

    private Rigidbody2D ObjectBody;
    private Collider2D ObjectCollider;

    void Start()
    {
        ObjectBody = GetComponent<Rigidbody2D>();
        ObjectCollider = GetComponent<Collider2D>();
    }

    private void OnMouseDown()
    {
        // NEW: Check if this specific object is tagged as a DropZone
        if (gameObject.CompareTag("DropZone"))
        {
            if (heldObject != null)
            {
                Debug.Log($"Clicked on drop zone {gameObject.name}! Telling {heldObject.name} to drop here.");

                pickupobject heldScript = heldObject.GetComponent<pickupobject>();
                if (heldScript != null)
                {
                    // Dynamically pass THIS clicked square as the target destination
                    heldScript.InitiateDropFromBlock(transform);
                }
            }
            return;
        }

        // STANDARD CLICK (For picking up items)
        if (heldObject == null && !held && !drop)
        {
            Pressed = true;
            Debug.Log($"You clicked directly on {gameObject.name}!");
            if (ObjectBody != null)
            {
                ObjectBody.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }

    // NEW: Accepts a dynamic target transform parameter
    public void InitiateDropFromBlock(Transform targetDestination)
    {
        UnityEngine.Debug.Log($"Drop state activated. Target destination set to {targetDestination.name}");

        PlaceBlockTransform = targetDestination; // Store the clicked square as our target destination

        if (ObjectCollider != null)
        {
            ObjectCollider.enabled = true;
            ObjectCollider.isTrigger = true;
        }

        release = false;
        Pressed = false;
        held = false;
        Tool = gameObject.CompareTag("Tools");
        block = gameObject.CompareTag("Block");
        drop = true;
        PlayerMove = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (drop && other.transform == PlaceBlockTransform)
        {
            ExecuteDropRelease();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (drop && collision.transform == PlaceBlockTransform)
        {
            ExecuteDropRelease();
        }
    }

    private void ExecuteDropRelease()
    {
        UnityEngine.Debug.Log($"{gameObject.name} released and ties to player severed.");

        transform.SetParent(null);
        transform.rotation = Quaternion.identity;

        if (ObjectBody != null)
        {
            ObjectBody.linearVelocity = Vector2.zero;
            ObjectBody.angularVelocity = 0f;
            ObjectBody.bodyType = RigidbodyType2D.Static;
        }

        if (ObjectCollider != null)
        {
            ObjectCollider.enabled = true;
            ObjectCollider.isTrigger = false;
        }

        heldObject = null;
        PlaceBlockTransform = null; // Clear the temporary target address out

        drop = false;
        release = false;
        held = false;
        Tool = false;
        block = false;
        PlayerMove = false;
    }

    void Update()
    {
        // SAFETY: If this object is tagged as a drop zone, skip movement logic
        if (gameObject.CompareTag("DropZone")) return;

        Vector3 facingDirection = Vector3.right;
        if (PlayerTransform != null)
        {
            facingDirection = PlayerTransform.localScale.x >= 0 ? Vector3.right : Vector3.left;
        }

        if (Pressed && gameObject.CompareTag("Tools") && !release)
        {
            UnityEngine.Debug.Log("pickuptool");
            if (ObjectBody != null) { ObjectBody.bodyType = RigidbodyType2D.Kinematic; }
            if (ObjectCollider != null) { ObjectCollider.enabled = false; }

            heldObject = gameObject;
            release = true;
            Pressed = false;
            held = true;
            Tool = true;
            drop = false;
            block = false;
            PlayerMove = true;
        }
        else if (Pressed && gameObject.CompareTag("Block") && !release)
        {
            UnityEngine.Debug.Log("pickupBlock");
            if (ObjectBody != null) { ObjectBody.bodyType = RigidbodyType2D.Kinematic; }
            if (ObjectCollider != null) { ObjectCollider.enabled = false; }

            heldObject = gameObject;
            release = true;
            Pressed = false;
            held = true;
            Tool = false;
            block = true;
            drop = false;
            PlayerMove = true;
        }

        // --- MOVEMENT LOGIC ---
        if (held && Tool && !PlayerMove)
        {
            Vector3 targetHoldPos = PlayerTransform.position + (facingDirection * holdOffsetDistance);
            ToolTransform.position = Vector3.MoveTowards(ToolTransform.position, targetHoldPos, speed * Time.deltaTime);
        }
        else if (held && block && !PlayerMove)
        {
            Vector3 targetHoldPos = PlayerTransform.position + (facingDirection * holdOffsetDistance);
            BlockTransform.position = Vector3.MoveTowards(BlockTransform.position, targetHoldPos, speed * Time.deltaTime);
        }
        else if (held && PlayerMove && Tool)
        {
            PlayerTransform.position = Vector3.MoveTowards(PlayerTransform.position, ToolTransform.position, speed * Time.deltaTime);
            if (Vector3.Distance(PlayerTransform.position, ToolTransform.position) < 0.01f)
            {
                PlayerMove = false;
                Vector3 targetHoldPos = PlayerTransform.position + (facingDirection * holdOffsetDistance);
                ToolTransform.position = targetHoldPos;
            }
        }
        else if (held && PlayerMove && block)
        {
            PlayerTransform.position = Vector3.MoveTowards(PlayerTransform.position, BlockTransform.position, speed * Time.deltaTime);
            if (Vector3.Distance(PlayerTransform.position, BlockTransform.position) < 0.01f)
            {
                PlayerMove = false;
                Vector3 targetHoldPos = PlayerTransform.position + (facingDirection * holdOffsetDistance);
                BlockTransform.position = targetHoldPos;
            }
        }
        else if (drop && PlaceBlockTransform != null)
        {
            Transform transformToMove = Tool ? ToolTransform : BlockTransform;

            transformToMove.position = Vector3.MoveTowards(transformToMove.position, PlaceBlockTransform.position, speed * Time.deltaTime);
            PlayerTransform.position = Vector3.MoveTowards(PlayerTransform.position, PlaceBlockTransform.position, speed * Time.deltaTime);

            if (Vector3.Distance(transformToMove.position, PlaceBlockTransform.position) < 0.1f ||
                Vector3.Distance(PlayerTransform.position, PlaceBlockTransform.position) < 0.1f)
            {
                ExecuteDropRelease();
            }
        }
    }
}
