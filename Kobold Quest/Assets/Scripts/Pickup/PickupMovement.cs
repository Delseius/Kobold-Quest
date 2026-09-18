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

        Transform item =
            pickup.ConsumableTransformReference;

        Transform block =
            pickup.BlockTransformReference;

        if (player == null)
        {
            return;
        }

        Vector3 facingDirection = Vector3.right;

        if (player.localScale.x < 0)
        {
            facingDirection = Vector3.left;
        }

        // Holding an Item and player is ready
        if (
            pickup.held &&
            !pickup.block &&
            !pickup.PlayerMove
        )
        {
            Vector3 targetHoldPos =
                player.position +
                (facingDirection * pickup.HoldOffsetDistance);

            if (item != null)
            {
                item.position =
                    Vector3.MoveTowards(
                        item.position,
                        targetHoldPos,
                        pickup.speed * Time.deltaTime
                    );
            }
        }

        // Holding a Block and player is ready
        else if (
            pickup.held &&
            pickup.block &&
            !pickup.PlayerMove
        )
        {
            Vector3 targetHoldPos =
                player.position +
                (facingDirection * pickup.HoldOffsetDistance);

            if (block != null)
            {
                block.position =
                    Vector3.MoveTowards(
                        block.position,
                        targetHoldPos,
                        pickup.speed * Time.deltaTime
                    );
            }
        }

        // Move player toward Item after pickup
        else if (
            pickup.held &&
            pickup.PlayerMove &&
            !pickup.block
        )
        {
            if (item == null)
            {
                return;
            }

            player.position =
                Vector3.MoveTowards(
                    player.position,
                    item.position,
                    pickup.speed * Time.deltaTime
                );

            if (
                Vector3.Distance(
                    player.position,
                    item.position
                ) < 0.05f
            )
            {
                pickup.PlayerMove = false;

                Vector3 targetHoldPos =
                    player.position +
                    (facingDirection * pickup.HoldOffsetDistance);

                item.position = targetHoldPos;
            }
        }

        // Move player toward Block after pickup
        else if (
            pickup.held &&
            pickup.PlayerMove &&
            pickup.block
        )
        {
            if (block == null)
            {
                return;
            }

            player.position =
                Vector3.MoveTowards(
                    player.position,
                    block.position,
                    pickup.speed * Time.deltaTime
                );

            if (
                Vector3.Distance(
                    player.position,
                    block.position
                ) < 0.05f
            )
            {
                pickup.PlayerMove = false;

                Vector3 targetHoldPos =
                    player.position +
                    (facingDirection * pickup.HoldOffsetDistance);

                block.position = targetHoldPos;
            }
        }

        // Move object and player toward drop zone
        else if (
            pickup.drop &&
            pickup.PlaceBlockTransform != null
        )
        {
            Transform transformToMove =
                pickup.block
                    ? block
                    : item;

            if (transformToMove == null)
            {
                return;
            }

            transformToMove.position =
                Vector3.MoveTowards(
                    transformToMove.position,
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
                    transformToMove.position,
                    pickup.CalculatedDropTarget
                ) < 0.1f
            )
            {
                pickup.ExecuteDropRelease();
            }
        }
    }
}