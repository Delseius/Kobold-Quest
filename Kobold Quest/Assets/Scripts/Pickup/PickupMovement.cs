using UnityEngine;

public class PickupMovement
{
    private pickupobject pickup;

    public PickupMovement(pickupobject pickup)
    {
        this.pickup = pickup;
    }

    public void UpdateMovement()
    {
        Transform player =
            pickup.PlayerTransformReference;

        if (player == null)
        {
            return;
        }

        // Always move the actual object stored in heldObject.
        // This makes every Item behave identically regardless of
        // whether it is a pickaxe, shovel, hoe, hammer or mushroom.
        GameObject heldGameObject =
            pickupobject.heldObject;

        Transform heldTransform =
            heldGameObject != null
                ? heldGameObject.transform
                : null;

        Vector3 facingDirection = Vector3.right;

        if (player.localScale.x < 0)
        {
            facingDirection = Vector3.left;
        }

        Vector3 targetHoldPos =
            player.position +
            (facingDirection * pickup.HoldOffsetDistance);

        // Move the Kobold toward the exact held object after pickup.
        if (
            pickup.held &&
            pickup.PlayerMove &&
            heldTransform != null
        )
        {
            player.position =
                Vector3.MoveTowards(
                    player.position,
                    heldTransform.position,
                    pickup.speed * Time.deltaTime
                );

            if (
                Vector3.Distance(
                    player.position,
                    heldTransform.position
                ) < 0.05f
            )
            {
                pickup.PlayerMove = false;
                heldTransform.position = targetHoldPos;
            }

            return;
        }

        // Once the Kobold reaches the object, keep the exact object
        // attached to the Kobold's hand position every frame.
        if (
            pickup.held &&
            !pickup.PlayerMove &&
            heldTransform != null
        )
        {
            heldTransform.position =
                Vector3.MoveTowards(
                    heldTransform.position,
                    targetHoldPos,
                    pickup.speed * Time.deltaTime
                );

            return;
        }

        // During a drop, still use the actual held object.
        if (
            pickup.drop &&
            pickup.PlaceBlockTransform != null &&
            heldTransform != null
        )
        {
            heldTransform.position =
                Vector3.MoveTowards(
                    heldTransform.position,
                    pickup.CalculatedDropTarget,
                    pickup.speed * Time.deltaTime
                );

            player.position =
                Vector3.MoveTowards(
                    player.position,
                    pickup.PlaceBlockTransform.position,
                    pickup.speed * Time.deltaTime
                );

            if (
                Vector3.Distance(
                    player.position,
                    pickup.PlaceBlockTransform.position
                ) < 0.1f
                ||
                Vector3.Distance(
                    heldTransform.position,
                    pickup.CalculatedDropTarget
                ) < 0.1f
            )
            {
                pickup.ExecuteDropRelease();
            }
        }
    }
}