using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Attach to the Surface Entrance. When the player interacts with it
// (presses the interact key nearby, or clicks it while nearby):
// - holding something -> the held object is sent to the surface (deleted)
// - holding nothing   -> the player comes back holding wood
public class SurfaceEntrance : MonoBehaviour
{
    [Header("Surface Entrance")]
    [SerializeField]
    [Tooltip("The item the player receives when empty-handed (e.g. the Log prefab).")]
    private GameObject woodPrefab;

    [SerializeField]
    [Tooltip("How close (in world units) the player must be to interact.")]
    private float interactRadius = 2.5f;

    [SerializeField]
    private Key interactKey = Key.Q;

    private Transform player;
    private Collider2D entranceCollider;

    private void Awake()
    {
        entranceCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (
            Keyboard.current != null &&
            Keyboard.current[interactKey].wasPressedThisFrame
        )
        {
            TryInteract();
        }
    }

    private void OnMouseDown()
    {
        TryInteract();
    }

    public bool TryInteract()
    {
        Transform playerTransform = GetPlayer();

        if (playerTransform == null)
        {
            Debug.LogWarning(
                "Surface Entrance: no GameObject with the Player tag was found."
            );
            return false;
        }

        if (!IsPlayerInRange(playerTransform))
        {
            return false;
        }

        if (pickupobject.heldObject != null)
        {
            SendHeldObjectToSurface();
        }
        else
        {
            GiveWood(playerTransform);
        }

        return true;
    }

    private void SendHeldObjectToSurface()
    {
        GameObject held = pickupobject.heldObject;

        pickupobject heldScript =
            held.GetComponent<pickupobject>();

        if (heldScript != null)
        {
            heldScript.ClearHeldState();
        }

        pickupobject.heldObject = null;

        Debug.Log($"{held.name} was sent to the surface.");

        Destroy(held);
    }

    private void GiveWood(Transform playerTransform)
    {
        if (woodPrefab == null)
        {
            Debug.LogWarning(
                "Surface Entrance: assign a Wood Prefab in the Inspector."
            );
            return;
        }

        GameObject wood =
            Instantiate(
                woodPrefab,
                playerTransform.position,
                Quaternion.identity
            );

        pickupobject woodPickup =
            wood.GetComponent<pickupobject>();

        if (woodPickup == null)
        {
            Debug.LogWarning(
                $"Surface Entrance: {woodPrefab.name} has no pickupobject component, " +
                "so the player can't hold it."
            );
            return;
        }

        woodPickup.SetPlayerTransform(playerTransform);

        // Reserve the player's hands now so a second press this frame
        // doesn't spawn more wood.
        pickupobject.heldObject = wood;

        StartCoroutine(PickUpNextFrame(woodPickup));

        Debug.Log("The player came back from the surface holding wood.");
    }

    // pickupobject.Start() resets the held state, so wait until it has run
    // before telling the new wood to be picked up.
    private IEnumerator PickUpNextFrame(pickupobject woodPickup)
    {
        yield return null;

        if (woodPickup == null)
        {
            yield break;
        }

        pickupobject.heldObject = null;
        woodPickup.InitiatePickup();
    }

    // The interact area is the entrance's collider box grown by
    // interactRadius on every side (the yellow box drawn when selected).
    // Measuring from the centre wouldn't work: the solid collider stops
    // the player from ever getting close to the middle.
    private bool IsPlayerInRange(Transform playerTransform)
    {
        Bounds area = GetInteractArea();
        Vector3 position = playerTransform.position;

        return
            position.x >= area.min.x && position.x <= area.max.x &&
            position.y >= area.min.y && position.y <= area.max.y;
    }

    private Bounds GetInteractArea()
    {
        Collider2D areaCollider =
            entranceCollider != null
                ? entranceCollider
                : GetComponent<Collider2D>();

        Bounds area =
            areaCollider != null
                ? areaCollider.bounds
                : new Bounds(transform.position, Vector3.zero);

        area.Expand(new Vector3(interactRadius * 2f, interactRadius * 2f, 0f));

        return area;
    }

    private Transform GetPlayer()
    {
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        return player;
    }

    private void OnDrawGizmosSelected()
    {
        Bounds area = GetInteractArea();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(area.center, area.size);
    }
}
