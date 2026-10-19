using UnityEngine;

public class PickupMovement
{
    private readonly pickupobject pickup;

    public PickupMovement(pickupobject pickup)
    {
        this.pickup = pickup;
    }

    public void UpdateMovement()
    {
        Transform player = pickup.PlayerTransformReference;
        Transform heldTransform = pickup.transform;

        if (player == null || heldTransform == null)
        {
            return;
        }

        Vector3 facingDirection =
            player.localScale.x < 0 ? Vector3.left : Vector3.right;

        // While approaching the picked-up object, move the PLAYER toward
        // this exact object. Do not use ToolTransform/BlockTransform here:
        // those references may point at a different object.
        if (pickup.held && pickup.PlayerMove)
        {
            player.position = Vector3.MoveTowards(
                player.position,
                heldTransform.position,
                pickup.speed * Time.deltaTime
            );

            if (Vector3.Distance(player.position, heldTransform.position) < 0.05f)
            {
                pickup.PlayerMove = false;
                heldTransform.position =
                    player.position + facingDirection * pickup.HoldOffsetDistance;
            }

            return;
        }

        // Items and Equipment follow the player at the hand offset.
        if (pickup.held && pickup.Tool && !pickup.PlayerMove)
        {
            heldTransform.position = Vector3.MoveTowards(
                heldTransform.position,
                player.position + facingDirection * pickup.HoldOffsetDistance,
                pickup.speed * Time.deltaTime
            );

            return;
        }

        // Blocks follow the same approach/hold movement once unlocked.
        if (pickup.held && pickup.block && !pickup.PlayerMove)
        {
            heldTransform.position = Vector3.MoveTowards(
                heldTransform.position,
                player.position + facingDirection * pickup.HoldOffsetDistance,
                pickup.speed * Time.deltaTime
            );

            return;
        }

        // Move the held object and player toward a clicked drop destination.
        if (pickup.drop && pickup.PlaceBlockTransform != null)
        {
            heldTransform.position = Vector3.MoveTowards(
                heldTransform.position,
                pickup.CalculatedDropTarget,
                pickup.speed * Time.deltaTime
            );

            player.position = Vector3.MoveTowards(
                player.position,
                pickup.PlaceBlockTransform.position,
                pickup.speed * Time.deltaTime
            );

            if (
                Vector3.Distance(player.position, pickup.PlaceBlockTransform.position) < 0.1f ||
                Vector3.Distance(heldTransform.position, pickup.CalculatedDropTarget) < 0.1f
            )
            {
                pickup.ExecuteDropRelease();
            }
        }
    }
}