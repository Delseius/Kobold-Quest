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
    private Vector3 calculatedDropTarget;

    [SerializeField] private Collider2D normalCollider;
    [SerializeField] private Collider2D temporaryCollider;
    [SerializeField] private float holdOffsetDistance = 0.0f;
    [SerializeField] private float verticalDropOffset = 0.1f;
    [SerializeField] private float pickupRadius = 1.0f; // NEW: Interaction range for pressing the 'C' key

    bool Pressed = false;
    bool release = false;
    bool held = false;
    bool Tool = false;
    bool block = false;
    bool drop = false;
    bool PlayerMove = false;
    //heldObject == null;

    public float speed = 5.5F;
    public static GameObject heldObject;
    

    private Rigidbody2D ObjectBody;
    private Collider2D ObjectCollider;
    private SpriteRenderer ObjectSpriteRenderer; // NEW: Tracks this item's sprite renderer

    private void Awake()
    {
        // Explicitly sets the object to null before anything else runs
        heldObject = null;
    }
    void Start()
    {
        ObjectBody = GetComponent<Rigidbody2D>();
        ObjectCollider = GetComponent<Collider2D>();
        ObjectSpriteRenderer = GetComponent<SpriteRenderer>(); // NEW: Cache the sprite renderer
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

    private void InitiatePickup()
    {
        Pressed = true;
        Debug.Log($"Pickup sequence triggered for: {gameObject.name}");
        if (ObjectBody != null)
        {
            ObjectBody.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    // NEW: Accepts a dynamic target transform parameter
    public void InitiateDropFromBlock(Transform targetDestination)
    {
        UnityEngine.Debug.Log($"Drop state activated. Target destination set to {targetDestination.name}");

        PlaceBlockTransform = targetDestination; // Store the clicked square as our target destination
        //calculatedDropTarget = PlaceBlockTransform.position + new Vector3(0, verticalDropOffset, 0);
        // CRUCIAL: Calculate the exact center of the block, then shift up vertically
        calculatedDropTarget = new Vector3(PlaceBlockTransform.position.x, PlaceBlockTransform.position.y + verticalDropOffset, PlaceBlockTransform.position.z);

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
    private void DropAtFeet()
    {
        UnityEngine.Debug.Log($"E Pressed! New Input System dropping {gameObject.name} at player's feet.");
        // NEW: If dropped at feet, just reset it to standard foreground sorting layer
        if (ObjectSpriteRenderer != null)
        {
            ObjectSpriteRenderer.sortingOrder = 5;
        }

        calculatedDropTarget = PlayerTransform.position;
        ExecuteDropRelease();


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
        transform.position = calculatedDropTarget;
        // NEW LAYER SORTING LOGIC:
        if (ObjectSpriteRenderer != null && PlaceBlockTransform != null)
        {
            SpriteRenderer blockRenderer = PlaceBlockTransform.GetComponent<SpriteRenderer>();
            if (blockRenderer != null)
            {
                // Force this item's order to be exactly 1 layer higher than the block it landed on
                ObjectSpriteRenderer.sortingOrder = blockRenderer.sortingOrder + 1;
                Debug.Log($"Adjusted {gameObject.name} layer sorting order to {ObjectSpriteRenderer.sortingOrder} to sit over block.");
            }
            else
            {
                ObjectSpriteRenderer.sortingOrder = 5; // Default safe backup layer if block has no renderer
            }
        }


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
        // NEW INPUT SYSTEM CHECK: Evaluates device input frames natively
        if (Keyboard.current != null)
        {
            if (held && !PlayerMove && Keyboard.current.eKey.wasPressedThisFrame)
            {
                DropAtFeet();
                return;
            }

            if (heldObject == null && !held && !drop && Keyboard.current.cKey.wasPressedThisFrame)
            {
                if (PlayerTransform != null)
                {
                    float distanceToPlayer = Vector3.Distance(transform.position, PlayerTransform.position);
                    if (distanceToPlayer <= pickupRadius)
                    {
                        Debug.Log($"C Key Pressed near {gameObject.name}! (Distance: {distanceToPlayer})");
                        InitiatePickup();
                    }
                }
            }
        }
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

            // NEW: Make item render way in front while the player carries it around
            if (ObjectSpriteRenderer != null) { ObjectSpriteRenderer.sortingOrder = 15; }

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

            // NEW: Make item render way in front while the player carries it around
            if (ObjectSpriteRenderer != null) { ObjectSpriteRenderer.sortingOrder = 15; }

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
            if (Vector3.Distance(PlayerTransform.position, ToolTransform.position) < 0.2f)
            {
                PlayerMove = false;
                Vector3 targetHoldPos = PlayerTransform.position + (facingDirection * holdOffsetDistance);
                ToolTransform.position = targetHoldPos;
            }
        }
        else if (held && PlayerMove && block)
        {
            PlayerTransform.position = Vector3.MoveTowards(PlayerTransform.position, BlockTransform.position, speed * Time.deltaTime);
            if (Vector3.Distance(PlayerTransform.position, BlockTransform.position) < 0.2f)
            {
                PlayerMove = false;
                Vector3 targetHoldPos = PlayerTransform.position + (facingDirection * holdOffsetDistance);
                BlockTransform.position = targetHoldPos;
            }
        }
        else if (drop && PlaceBlockTransform != null)
        {
            Transform transformToMove = Tool ? ToolTransform : BlockTransform;

            //transformToMove.position = Vector3.MoveTowards(transformToMove.position, PlaceBlockTransform.position, speed * Time.deltaTime);
            transformToMove.position = Vector3.MoveTowards(transformToMove.position, calculatedDropTarget, speed * Time.deltaTime);

            PlayerTransform.position = Vector3.MoveTowards(PlayerTransform.position, PlaceBlockTransform.position, speed * Time.deltaTime);

            /*if (Vector3.Distance(transformToMove.position, calculatedDropTarget) < 0.2f ||
                Vector3.Distance(PlayerTransform.position, PlaceBlockTransform.position) < 0.2f)
            {
                ExecuteDropRelease();
            }*/
            if (Vector3.Distance(PlayerTransform.position, PlaceBlockTransform.position) < 0.1f ||
                Vector3.Distance(transformToMove.position, calculatedDropTarget) < 0.1f)
            {
                ExecuteDropRelease();
            }
        }
    }
}

