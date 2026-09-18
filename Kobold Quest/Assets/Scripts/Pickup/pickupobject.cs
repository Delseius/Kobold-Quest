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

    // A block starts locked. It becomes pickupable after a pickaxe
    // has been used on/clicked on that block.
    private bool blockPickupUnlocked = false;

    public bool IsBlockPickupUnlocked
    {
        get { return blockPickupUnlocked; }
    }

    // Remembers the last block clicked so F can activate it with a pickaxe.
    private static pickupobject selectedBlock;


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

    public static Transform FindPlayerTransform()
    {
        pickupobject[] pickupObjects =
            FindObjectsOfType<pickupobject>();
            
            

        foreach (pickupobject candidate in pickupObjects)
        {
            if (candidate.PlayerTransformReference != null)
            {
                return candidate.PlayerTransformReference;
            }
        }

        return null;
    }

    public static pickupobject FindNearestPickupable(
        Transform player,
        float radius
    )
    {
        if (player == null)
        {
            return null;
        }

        pickupobject[] pickupObjects =
            FindObjectsOfType<pickupobject>();

        pickupobject nearest = null;
        float nearestDistance = radius;

        foreach (pickupobject candidate in pickupObjects)
        {
            if (
                candidate == null ||
                !candidate.gameObject.activeInHierarchy
            )
            {
                continue;
            }

            bool isItem =
                candidate.CompareTag("Item");

            bool isBlock =
                candidate.CompareTag("Block");

            if (!isItem && !isBlock)
            {
                continue;
            }

            // Blocks cannot be picked up until a pickaxe has activated them.
            if (isBlock && !candidate.IsBlockPickupUnlocked)
            {
                continue;
            }

            if (
                candidate.held ||
                candidate.drop ||
                candidate.release
            )
            {
                continue;
            }

            float distance =
                Vector3.Distance(
                    candidate.transform.position,
                    player.position
                );

            if (distance <= nearestDistance)
            {
                nearestDistance = distance;
                nearest = candidate;
            }
        }

        return nearest;
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
        ClearHeldState();
        if (
            heldObject != null &&
            !heldObject.activeInHierarchy
        )
        {
            heldObject = null;
        }

        Pressed = false;
        release = false;
        held = false;
        Tool = false;
        block = false;
        drop = false;
        PlayerMove = false;

        objectBody =
            GetComponent<Rigidbody2D>();

        objectCollider =
            GetComponent<Collider2D>();

        objectSpriteRenderer =
            GetComponent<SpriteRenderer>();

        pickupInput =
            new PickupInput(this);

        pickupMovement =
            new PickupMovement(this);

        pickupDropSystem =
            new PickupDropSystem(this);
    }

    private void OnMouseDown()
    {
        pickupInput.HandleMouseDown();
    }

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        pickupDropSystem.HandleTriggerEnter(other);
    }

    private void OnCollisionEnter2D(
        Collision2D collision
    )
    {
        pickupDropSystem.HandleCollisionEnter(collision);
    }

    public void InitiatePickup()
    {
        Pressed = true;

        UnityEngine.Debug.Log(
            $"Pickup sequence triggered for: {gameObject.name}"
        );

        if (objectBody != null)
        {
            objectBody.bodyType =
                RigidbodyType2D.Kinematic;
        }
    }

    public void InitiateDropFromBlock(
        Transform targetDestination
    )
    {
        pickupDropSystem.InitiateDropFromBlock(
            targetDestination
        );
    }

    public void DropAtFeet()
    {
        pickupDropSystem.DropAtFeet();
    }

    public void ExecuteDropRelease()
    {
        pickupDropSystem.ExecuteDropRelease();
    }

    // Unlocks a block after a pickaxe activates it.
    public void UnlockBlockForPickup()
    {
        if (!CompareTag("Block"))
        {
            return;
        }

        blockPickupUnlocked = true;
        selectedBlock = this;

        UnityEngine.Debug.Log(
            $"{gameObject.name} has been activated " +
            "by the pickaxe. The block can now " +
            "be picked up."
        );
    }
    // Checks whether the currently held object is a pickaxe.
    public static bool IsPickaxeHeld()
    {
        if (heldObject == null)
        {
            return false;
        }

        string heldName =
            heldObject.name.ToLowerInvariant();

        return heldName.Contains("pickaxe");
    }

    public static bool IsShovelHeld()
    {
        if (heldObject == null)
        {
            return false;
        }

        string heldName =
            heldObject.name.ToLowerInvariant();

        return heldName.Contains("shovel");
    }

    public static bool IsCorrectToolForBlock(GameObject blockObject)
    {
        if (
            blockObject == null ||
            !blockObject.CompareTag("Block")
        )
        {
            return false;
        }

        string blockName =
            blockObject.name.ToLowerInvariant();

        // Dirt blocks require a shovel.
        if (blockName.Contains("dirt"))
        {
            return IsShovelHeld();
        }

        // Stone blocks require a pickaxe.
        if (blockName.Contains("stone"))
        {
            return IsPickaxeHeld();
        }

        return false;
    }

    public static string GetHeldToolName()
    {
        if (heldObject == null)
        {
            return "No tool";
        }

        return heldObject.name;
    }

    public static bool IsCorrectToolHeld()
    {
        return IsPickaxeHeld() || IsShovelHeld();
    }

    public static void SelectBlock(
        pickupobject block
    )
    {
        if (
            block != null &&
            block.CompareTag("Block")
        )
        {
            selectedBlock = block;
        }
    }

    public static pickupobject GetSelectedBlock()
    {
        return selectedBlock;
    }

    public static void ClearSelectedBlock()
    {
        selectedBlock = null;
    }


    


    public void ClearHeldState()
    {
        if (heldObject == gameObject)
        {
            heldObject = null;
        }
        Pressed = false;
        held = false;
        release = false;
        Tool = false;
        block = false;
        drop = false;
        PlayerMove = false;
        PlaceBlockTransform = null;
        CalculatedDropTarget = Vector3.zero;
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
        if (!Pressed || release)
        {
            return;
        }

        bool isItem =
            gameObject.CompareTag("Item");

        bool isBlock =
            gameObject.CompareTag("Block");

        if (!isItem && !isBlock)
        {
            return;
        }

        // A block must first be activated by a pickaxe.
        if (isBlock && !blockPickupUnlocked)
        {
            Pressed = false;

            UnityEngine.Debug.Log(
                $"{gameObject.name} cannot be picked up yet. " +
                "Use a pickaxe on the block first."
            );

            return;
        }


        UnityEngine.Debug.Log(
            isBlock
                ? "pickupBlock"
                : "pickupItem"
        );

        if (objectBody != null)
        {
            objectBody.bodyType =
                RigidbodyType2D.Kinematic;
        }

        if (objectCollider != null)
        {
            objectCollider.enabled = false;
        }
        GridObject gridObject =
            GetComponent<GridObject>();

        if (gridObject != null)
        {
            gridObject.DisableGridObject();
        }

        if (objectSpriteRenderer != null)
        {
            objectSpriteRenderer.sortingOrder = 15;
        }

        heldObject = gameObject;

        release = true;
        Pressed = false;
        held = true;

        Tool = isItem;
        block = isBlock;

        drop = false;
        PlayerMove = true;
    }
}