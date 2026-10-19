using UnityEngine;

/// <summary>
/// Temporary diagnostics for pickup selection, held-item movement, and drop position.
/// Attach to each scene instance of a pickupable object during testing.
/// Logs observations only; it does not change pickup behaviour.
/// </summary>
[DisallowMultipleComponent]
public class PickupRuntimeDiagnostics : MonoBehaviour
{
    [SerializeField] private bool logWhilePickupIsActive = true;
    [SerializeField, Min(0.1f)] private float logIntervalSeconds = 0.5f;
    [SerializeField, Min(1)] private int framesToTraceAfterRelease = 4;

    private pickupobject pickup;
    private float nextLogTime;
    private string previousState;
    private bool wasHeld;
    private int releaseFramesRemaining;

    private void Awake()
    {
        pickup = GetComponent<pickupobject>();

        if (pickup == null)
        {
            Debug.LogWarning(
                $"[PickupDiagnostics] {name} has no pickupobject component.",
                this
            );
            enabled = false;
            return;
        }

        wasHeld = pickup.held || pickupobject.heldObject == gameObject;
    }

    private void Update()
    {
        if (pickup == null)
        {
            return;
        }

        string state = GetStateSummary();

        if (state != previousState)
        {
            Debug.Log($"[PickupDiagnostics] STATE {state}", this);
            previousState = state;
        }

        bool isHeldNow = pickup.held || pickupobject.heldObject == gameObject;

        if (wasHeld && !isHeldNow)
        {
            releaseFramesRemaining = framesToTraceAfterRelease;
            LogSnapshot("RELEASE_TRANSITION");
        }

        wasHeld = isHeldNow;

        if (releaseFramesRemaining > 0)
        {
            LogSnapshot($"POST_RELEASE_FRAME_{framesToTraceAfterRelease - releaseFramesRemaining + 1}");
            releaseFramesRemaining--;
        }

        bool activePickup =
            pickup.Pressed ||
            pickup.held ||
            pickup.release ||
            pickup.PlayerMove ||
            pickup.drop ||
            pickupobject.heldObject == gameObject;

        if (
            logWhilePickupIsActive &&
            activePickup &&
            Time.unscaledTime >= nextLogTime
        )
        {
            nextLogTime = Time.unscaledTime + logIntervalSeconds;
            LogSnapshot("ACTIVE");
        }
    }

    private string GetStateSummary()
    {
        Transform player = pickup.PlayerTransformReference;
        float distance = player != null
            ? Vector3.Distance(transform.position, player.position)
            : -1f;

        return
            $"{name}: Pressed={pickup.Pressed}, Held={pickup.held}, " +
            $"PlayerMove={pickup.PlayerMove}, Tool={pickup.Tool}, " +
            $"Block={pickup.block}, Release={pickup.release}, Drop={pickup.drop}, " +
            $"isStaticHeldObject={pickupobject.heldObject == gameObject}, " +
            $"distanceToPlayer={(distance >= 0f ? distance.ToString("F3") : "no player")}";
    }

    private void LogSnapshot(string phase)
    {
        Transform player = pickup.PlayerTransformReference;
        Transform tool = pickup.ToolTransformReference;
        Rigidbody2D body = pickup.ObjectBody;

        string playerPosition = player != null
            ? player.position.ToString("F3")
            : "NULL";

        string toolPosition = tool != null
            ? tool.position.ToString("F3")
            : "NULL";

        float distance = player != null
            ? Vector3.Distance(transform.position, player.position)
            : -1f;

        string bodyType = body != null
            ? body.bodyType.ToString()
            : "NO Rigidbody2D";

        Debug.Log(
            $"[PickupDiagnostics] {phase} item={name}; " +
            $"parent={(transform.parent != null ? transform.parent.name : "NULL")}; " +
            $"itemPos={transform.position:F3}; " +
            $"dropTarget={pickup.CalculatedDropTarget:F3}; " +
            $"heldObject={(pickupobject.heldObject != null ? pickupobject.heldObject.name : "NULL")}; " +
            $"playerPos={playerPosition}; " +
            $"toolRef={(tool != null ? tool.name : "NULL")}; toolPos={toolPosition}; " +
            $"distance={(distance >= 0f ? distance.ToString("F3") : "no player")}; " +
            $"bodyType={bodyType}; simulated={(body != null && body.simulated)}; " +
            $"tag={tag}; state=[{GetStateSummary()}]",
            this
        );
    }
}
