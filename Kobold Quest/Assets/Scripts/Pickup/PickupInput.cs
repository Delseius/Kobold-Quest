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

        // Every pickupable object uses the same heldObject reference.
        // The clicked leaf object becomes the held object directly.
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

        // E = Drop held object at player's feet.
        if (
            pickup.held &&
            !pickup.PlayerMove &&
            Keyboard.current.eKey.wasPressedThisFrame
        )
        {
            pickup.DropAtFeet();
            return;
        }

        // F = Use the currently held object.
        if (
            pickup.held &&
            !pickup.PlayerMove &&
            pickupobject.heldObject != null &&
            Keyboard.current.fKey.wasPressedThisFrame
        )
        {
            UseHeldItem();
            return;
        }

        // C = Pick up the nearest object represented by this
        // pickupobject component.
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

    private void UseHeldItem()
    {
        GameObject heldObject =
            pickupobject.heldObject;

        if (heldObject == null)
        {
            return;
        }

        IUsable usable =
            heldObject.GetComponent<IUsable>();

        if (usable == null)
        {
            UnityEngine.Debug.Log(
                $"{heldObject.name} cannot be used."
            );

            return;
        }

        Transform player =
            pickup.PlayerTransformReference;

        if (player == null)
        {
            return;
        }

        GameObject user =
            player.gameObject;

        bool consumed =
            usable.Use(user);

        if (consumed)
        {
            pickup.ClearHeldState();
        }
    }
}