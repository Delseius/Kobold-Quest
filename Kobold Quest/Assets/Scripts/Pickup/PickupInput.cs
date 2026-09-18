
using UnityEngine;
using UnityEngine.InputSystem;

public class PickupInput
{
    private pickupobject pickup;

    public PickupInput(pickupobject pickup)
    {
        this.pickup = pickup;
    }

    public void HandleMouseDown()
    {
        GameObject currentObject = pickup.gameObject;

        if (currentObject.CompareTag("DropZone"))
        {
            if (pickupobject.heldObject != null)
            {
                UnityEngine.Debug.Log(
                    $"Clicked on drop zone {currentObject.name}! " +
                    $"Telling {pickupobject.heldObject.name} to drop here."
                );

                pickupobject heldScript =
                    pickupobject.heldObject.GetComponent<pickupobject>();

                if (heldScript != null)
                {
                    heldScript.InitiateDropFromBlock(
                        currentObject.transform
                    );
                }
            }

            return;
        }

        if (
            pickupobject.heldObject == null &&
            !pickup.held &&
            !pickup.drop
        )
        {
            pickup.Pressed = true;

            UnityEngine.Debug.Log(
                $"You clicked directly on {currentObject.name}!"
            );

            if (pickup.ObjectBody != null)
            {
                pickup.ObjectBody.bodyType =
                    RigidbodyType2D.Kinematic;
            }
        }
    }

    public void HandleKeyboardInput()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        // E = Drop at player's feet
        if (
            pickup.held &&
            !pickup.PlayerMove &&
            Keyboard.current.eKey.wasPressedThisFrame
        )
        {
            pickup.DropAtFeet();
            return;
        }

        // C = Pick up nearby object
        if (
            pickupobject.heldObject == null &&
            !pickup.held &&
            !pickup.drop &&
            Keyboard.current.cKey.wasPressedThisFrame
        )
        {
            Transform player =
                pickup.PlayerTransformReference;

            if (player != null)
            {
                float distanceToPlayer =
                    Vector3.Distance(
                        pickup.transform.position,
                        player.position
                    );

                if (distanceToPlayer <= pickup.PickupRadius)
                {
                    UnityEngine.Debug.Log(
                        $"C Key Pressed near " +
                        $"{pickup.gameObject.name}! " +
                        $"(Distance: {distanceToPlayer})"
                    );

                    pickup.InitiatePickup();
                }
            }
        }
    }
}