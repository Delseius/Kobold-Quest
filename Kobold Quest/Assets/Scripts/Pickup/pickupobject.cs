

using UnityEngine;

public class pickupobject : MonoBehaviour
{
    [SerializeField] private Transform PlayerTransform;
    [SerializeField] private Transform BlockTransform;
    [SerializeField] private Transform ToolTransform;
    [SerializeField] private Transform ConsumableTransform;

    private Transform placeBlockTransform;
    private Vector3 calculatedDropTarget;

    [SerializeField] private float holdOffsetDistance = 1.2f;
    [SerializeField] private float verticalDropOffset = 0.6f;
    [SerializeField] private float pickupRadius = 2.0f;

    public float speed = 5.5F;

    public static GameObject heldObject;

    private Rigidbody2D objectBody;
    private Collider2D objectCollider;
    private SpriteRenderer objectSpriteRenderer;

    public bool Pressed { get; set; }
    public bool release { get; set; }
    public bool held { get; set; }
    public bool Tool { get; set; }
    public bool block { get; set; }
    public bool drop { get; set; }
    public bool PlayerMove { get; set; }

    public Transform PlaceBlockTransform
    {
        get { return placeBlockTransform; }
        set { placeBlockTransform = value; }
    }

    public Vector3 CalculatedDropTarget
    {
        get { return calculatedDropTarget; }
        set { calculatedDropTarget = value; }
    }

    public Transform PlayerTransformReference
    {
        get { return PlayerTransform; }
    }

    public Transform BlockTransformReference
    {
        get { return BlockTransform; }
    }

    public Transform ToolTransformReference
    {
        get { return ToolTransform; }
    }

    public Transform ConsumableTransformReference
    {
        get { return ConsumableTransform; }
    }

    public float HoldOffsetDistance
    {
        get { return holdOffsetDistance; }
    }

    public float VerticalDropOffset
    {
        get { return verticalDropOffset; }
    }

    public float PickupRadius
    {
        get { return pickupRadius; }
    }

    public Rigidbody2D ObjectBody
    {
        get { return objectBody; }
    }

    public Collider2D ObjectCollider
    {
        get { return objectCollider; }
    }

    public SpriteRenderer ObjectSpriteRenderer
    {
        get { return objectSpriteRenderer; }
    }

    private PickupInput pickupInput;
    private PickupMovement pickupMovement;
    private PickupDropSystem pickupDropSystem;

    private void Start()
    {
        objectBody = GetComponent<Rigidbody2D>();
        objectCollider = GetComponent<Collider2D>();
        objectSpriteRenderer = GetComponent<SpriteRenderer>();

        pickupInput = new PickupInput(this);
        pickupMovement = new PickupMovement(this);
        pickupDropSystem = new PickupDropSystem(this);
    }

    private void OnMouseDown()
    {
        pickupInput.HandleMouseDown();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        pickupDropSystem.HandleTriggerEnter(other);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        pickupDropSystem.HandleCollisionEnter(collision);
    }

    public void InitiatePickup()
    {
        Pressed = true;

        Debug.Log($"Pickup sequence triggered for: {gameObject.name}");

        if (objectBody != null)
        {
            objectBody.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    public void InitiateDropFromBlock(Transform targetDestination)
    {
        pickupDropSystem.InitiateDropFromBlock(targetDestination);
    }

    public void DropAtFeet()
    {
        pickupDropSystem.DropAtFeet();
    }

    public void ExecuteDropRelease()
    {
        pickupDropSystem.ExecuteDropRelease();
    }

    private void Update()
    {
        pickupInput.HandleKeyboardInput();

        if (gameObject.CompareTag("DropZone"))
        {
            return;
        }

        HandlePickupState();

        pickupMovement.UpdateMovement();
    }

    private void HandlePickupState()
    {
        if (Pressed && gameObject.CompareTag("Tools") && !release)
        {
            Debug.Log("pickuptool");

            if (objectBody != null)
            {
                objectBody.bodyType = RigidbodyType2D.Kinematic;
            }

            if (objectCollider != null)
            {
                objectCollider.enabled = false;
            }

            if (objectSpriteRenderer != null)
            {
                objectSpriteRenderer.sortingOrder = 15;
            }

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
            Debug.Log("pickupBlock");

            if (objectBody != null)
            {
                objectBody.bodyType = RigidbodyType2D.Kinematic;
            }

            if (objectCollider != null)
            {
                objectCollider.enabled = false;
            }

            if (objectSpriteRenderer != null)
            {
                objectSpriteRenderer.sortingOrder = 15;
            }

            heldObject = gameObject;

            release = true;
            Pressed = false;
            held = true;
            Tool = false;
            block = true;
            drop = false;
            PlayerMove = true;
        }
    }
}

