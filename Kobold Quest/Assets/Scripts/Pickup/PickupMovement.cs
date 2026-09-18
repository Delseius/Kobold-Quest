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

        Transform tool =
            pickup.ToolTransformReference;

        Transform block =
            pickup.BlockTransformReference;

        if (player == null)
        {
            return;
        }

        Vector3 facingDirection =
            Vector3.right;

        if (player.localScale.x < 0)
        {
            facingDirection =
                Vector3.left;
        }

        // Holding a tool and player is ready
        if (
            pickup.held &&
            pickup.Tool &&
            !pickup.PlayerMove
        )
        {
            Vector3 targetHoldPos =
                player.position +
                (
                    facingDirection *
                    pickup.HoldOffsetDistance
                );

            if (tool != null)
            {
                tool.position =
                    Vector3.MoveTowards(
                        tool.position,
                        targetHoldPos,
                        pickup.speed *
                        Time.deltaTime
                    );
            }
        }

        // Holding a block and player is ready
        else if (
            pickup.held &&
            pickup.block &&
            !pickup.PlayerMove
        )
        {
            Vector3 targetHoldPos =
                player.position +
                (
                    facingDirection *
                    pickup.HoldOffsetDistance
                );

            if (block != null)
            {
                block.position =
                    Vector3.MoveTowards(
                        block.position,
                        targetHoldPos,
                        pickup.speed *
                        Time.deltaTime
                    );
            }
        }

        // Move player toward tool after pickup
        else if (
            pickup.held &&
            pickup.PlayerMove &&
            pickup.Tool
        )
        {
            if (tool == null)
            {
                return;
            }

            player.position =
                Vector3.MoveTowards(
                    player.position,
                    tool.position,
                    pickup.speed *
                    Time.deltaTime
                );

            if (
                Vector3.Distance(
                    player.position,
                    tool.position
                ) < 0.05f
            )
            {
                pickup.PlayerMove = false;

                Vector3 targetHoldPos =
                    player.position +
                    (
                        facingDirection *
                        pickup.HoldOffsetDistance
                    );

                tool.position =
                    targetHoldPos;
            }
        }

        // Move player toward block after pickup
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
                    pickup.speed *
                    Time.deltaTime
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
                    (
                        facingDirection *
                        pickup.HoldOffsetDistance
                    );

                block.position =
                    targetHoldPos;
            }
        }

        // Move object and player toward drop zone
        else if (
            pickup.drop &&
            pickup.PlaceBlockTransform != null
        )
        {
            Transform transformToMove =
                pickup.Tool
                    ? tool
                    : block;

            if (transformToMove == null)
            {
                return;
            }

            transformToMove.position =
                Vector3.MoveTowards(
                    transformToMove.position,
                    pickup.CalculatedDropTarget,
                    pickup.speed *
                    Time.deltaTime
                );

            player.position =
                Vector3.MoveTowards(
                    player.position,
                    pickup.PlaceBlockTransform.position,
                    pickup.speed *
                    Time.deltaTime
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