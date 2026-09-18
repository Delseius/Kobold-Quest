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

    // This is the one source of truth for every picked-up object:
    // pickaxes, shovels, hoes, hammers, mushrooms and blocks.
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
        if (heldObject != null && heldObject != gameObject)
        {
            return;
        }

        Pressed = true;

        UnityEngine.Debug.Log(
            $"Pickup sequence triggered for: {gameObject.name}"
        );

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

    public void ClearHeldState()
    {
        if (heldObject == gameObject)
        {
            heldObject = null;
        }

        held = false;
        release = false;
        Tool = false;
        block = false;
        drop = false;
        PlayerMove = false;
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
        if (Pressed && !release)
        {
            bool isItem = gameObject.CompareTag("Item");
            bool isBlock = gameObject.CompareTag("Block");

            if (!isItem && !isBlock)
            {
                return;
            }

            UnityEngine.Debug.Log(
                isBlock ? "pickupBlock" : "pickupItem"
            );

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

            // Always store the actual clicked object.
            // No ToolTransform or ConsumableTransform is required.
            heldObject = gameObject;

            release = true;
            Pressed = false;
            held = true;
            Tool = false;
            block = isBlock;
            drop = false;

            // Move the Kobold toward this exact object first.
            PlayerMove = true;
        }
    }
}