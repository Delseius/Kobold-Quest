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
        Transform tool = pickup.ToolTransformReference;
        Transform block = pickup.BlockTransformReference;

        if (player == null)
        {
            return;
        }

        Vector3 facingDirection = player.localScale.x < 0 ? Vector3.left : Vector3.right;

        if (pickup.held && pickup.Tool && !pickup.PlayerMove)
        {
            if (tool != null)
            {
                Vector3 target = player.position + facingDirection * pickup.HoldOffsetDistance;
                tool.position = Vector3.MoveTowards(tool.position, target, pickup.speed * Time.deltaTime);
            }
        }
        else if (pickup.held && pickup.block && !pickup.PlayerMove)
        {
            if (block != null)
            {
                Vector3 target = player.position + facingDirection * pickup.HoldOffsetDistance;
                block.position = Vector3.MoveTowards(block.position, target, pickup.speed * Time.deltaTime);
            }
        }
        else if (pickup.held && pickup.PlayerMove && pickup.Tool)
        {
            if (tool == null)
            {
                return;
            }

            player.position = Vector3.MoveTowards(player.position, tool.position, pickup.speed * Time.deltaTime);
            if (Vector3.Distance(player.position, tool.position) < 0.05f)
            {
                pickup.PlayerMove = false;
                tool.position = player.position + facingDirection * pickup.HoldOffsetDistance;
            }
        }
        else if (pickup.held && pickup.PlayerMove && pickup.block)
        {
            if (block == null)
            {
                return;
            }

            player.position = Vector3.MoveTowards(player.position, block.position, pickup.speed * Time.deltaTime);
            if (Vector3.Distance(player.position, block.position) < 0.05f)
            {
                pickup.PlayerMove = false;
                block.position = player.position + facingDirection * pickup.HoldOffsetDistance;
            }
        }
        else if (pickup.drop && pickup.PlaceBlockTransform != null)
        {
            Transform transformToMove = pickup.Tool ? tool : block;
            if (transformToMove == null)
            {
                return;
            }

            transformToMove.position = Vector3.MoveTowards(transformToMove.position, pickup.CalculatedDropTarget, pickup.speed * Time.deltaTime);
            player.position = Vector3.MoveTowards(player.position, pickup.PlaceBlockTransform.position, pickup.speed * Time.deltaTime);

            if (Vector3.Distance(player.position, pickup.PlaceBlockTransform.position) < 0.1f ||
                Vector3.Distance(transformToMove.position, pickup.CalculatedDropTarget) < 0.1f)
            {
                pickup.ExecuteDropRelease();
            }
        }
    }
}