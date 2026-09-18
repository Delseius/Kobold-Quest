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

        // C = Pick up the nearest Item or Block around the player.
        // Only the pickupobject that has the PlayerTransform reference
        // should handle the C key, so every pickup does not trigger it.
        if (
            pickupobject.heldObject == null &&
            !pickup.held &&
            !pickup.drop &&
            Keyboard.current.cKey.wasPressedThisFrame
        )
        {
            Transform player =
                pickup.PlayerTransformReference;

            if (player == null)
            {
                player = pickupobject.FindPlayerTransform();
            }

            if (player == null)
            {
                UnityEngine.Debug.LogWarning(
                    "C pickup could not find the Player Transform. " +
                    "Assign PlayerTransform on one pickupobject."
                );

                return;
            }

            pickupobject nearest =
                pickupobject.FindNearestPickupable(
                    player,
                    pickup.PickupRadius
                );

            if (nearest != null)
            {
                float distanceToPlayer =
                    Vector3.Distance(
                        nearest.transform.position,
                        player.position
                    );

                UnityEngine.Debug.Log(
                    $"C Key Pressed: picking up nearest object " +
                    $"{nearest.gameObject.name} " +
                    $"(Distance: {distanceToPlayer})"
                );

                nearest.InitiatePickup();
            }
        }
    }
}