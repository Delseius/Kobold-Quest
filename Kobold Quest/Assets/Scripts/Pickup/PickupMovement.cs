using UnityEngine;

public class PickupMovement
{
    private readonly pickupobject pickup;
    private float nextDiagnosticTime;

    public PickupMovement(pickupobject pickup)
    {
        this.pickup = pickup;
    }

    public void UpdateMovement()
    {
        Transform player = pickup.PlayerTransformReference;
        Transform tool = pickup.ToolTransformReference;
        Transform block = pickup.BlockTransformReference;

        bool active = pickup.held || pickupobject.heldObject == pickup.gameObject ||
                      pickup.Pressed || pickup.release;
        bool toolFollow = pickup.held && pickup.Tool && !pickup.PlayerMove;
        bool blockFollow = pickup.held && pickup.block && !pickup.PlayerMove;
        bool toolPickup = pickup.held && pickup.PlayerMove && pickup.Tool;
        bool blockPickup = pickup.held && pickup.PlayerMove && pickup.block;
        bool dropBranch = pickup.drop && pickup.PlaceBlockTransform != null;

        string branch = toolFollow ? "TOOL_FOLLOW" :
                        blockFollow ? "BLOCK_FOLLOW" :
                        toolPickup ? "PLAYER_TO_TOOL" :
                        blockPickup ? "PLAYER_TO_BLOCK" :
                        dropBranch ? "DROP" : "NONE";

        bool logNow = active && Time.unscaledTime >= nextDiagnosticTime;
        if (logNow)
        {
            nextDiagnosticTime = Time.unscaledTime + 0.5f;
            Debug.Log(
                $"[PickupMovementDiag] BEFORE branch={branch}; object={pickup.name}#{pickup.GetInstanceID()}; " +
                $"held={pickup.held}; Tool={pickup.Tool}; block={pickup.block}; PlayerMove={pickup.PlayerMove}; " +
                $"Pressed={pickup.Pressed}; release={pickup.release}; drop={pickup.drop}; " +
                $"staticHeld={(pickupobject.heldObject != null ? pickupobject.heldObject.name + "#" + pickupobject.heldObject.GetInstanceID() : "NULL")}; " +
                $"playerRef={(player != null ? player.name + "#" + player.GetInstanceID() : "NULL")}; " +
                $"playerPos={(player != null ? player.position.ToString("F3") : "NULL")}; selfPos={pickup.transform.position:F3}; " +
                $"toolRef={(tool != null ? tool.name + "#" + tool.GetInstanceID() : "NULL")}; " +
                $"toolPos={(tool != null ? tool.position.ToString("F3") : "NULL")}; toolIsSelf={(tool == pickup.transform)}; " +
                $"blockRef={(block != null ? block.name + "#" + block.GetInstanceID() : "NULL")}; offset={pickup.HoldOffsetDistance:F3}",
                pickup
            );
        }

        if (player == null)
        {
            if (logNow) Debug.LogWarning($"[PickupMovementDiag] RETURN: player reference NULL for {pickup.name}#{pickup.GetInstanceID()}.", pickup);
            return;
        }

        Vector3 facingDirection = player.localScale.x < 0 ? Vector3.left : Vector3.right;

        if (toolFollow)
        {
            Vector3 target = player.position + facingDirection * pickup.HoldOffsetDistance;
            if (tool != null)
            {
                Vector3 oldPos = tool.position;
                tool.position = Vector3.MoveTowards(tool.position, target, pickup.speed * Time.deltaTime);
                if (logNow) Debug.Log($"[PickupMovementDiag] TOOL_FOLLOW old={oldPos:F3}; target={target:F3}; newTool={tool.position:F3}; self={pickup.transform.position:F3}.", pickup);
            }
            else if (logNow) Debug.LogWarning($"[PickupMovementDiag] TOOL_FOLLOW SKIPPED: tool reference NULL for {pickup.name}#{pickup.GetInstanceID()}.", pickup);
        }
        else if (blockFollow)
        {
            Vector3 target = player.position + facingDirection * pickup.HoldOffsetDistance;
            if (block != null)
            {
                Vector3 oldPos = block.position;
                block.position = Vector3.MoveTowards(block.position, target, pickup.speed * Time.deltaTime);
                if (logNow) Debug.Log($"[PickupMovementDiag] BLOCK_FOLLOW old={oldPos:F3}; target={target:F3}; newBlock={block.position:F3}; self={pickup.transform.position:F3}.", pickup);
            }
            else if (logNow) Debug.LogWarning($"[PickupMovementDiag] BLOCK_FOLLOW SKIPPED: block reference NULL for {pickup.name}#{pickup.GetInstanceID()}.", pickup);
        }
        else if (toolPickup)
        {
            if (tool == null)
            {
                if (logNow) Debug.LogWarning($"[PickupMovementDiag] PLAYER_TO_TOOL SKIPPED: tool reference NULL for {pickup.name}#{pickup.GetInstanceID()}.", pickup);
                return;
            }

            player.position = Vector3.MoveTowards(player.position, tool.position, pickup.speed * Time.deltaTime);
            if (Vector3.Distance(player.position, tool.position) < 0.05f)
            {
                pickup.PlayerMove = false;
                tool.position = player.position + facingDirection * pickup.HoldOffsetDistance;
            }
            if (logNow) Debug.Log($"[PickupMovementDiag] PLAYER_TO_TOOL result; player={player.position:F3}; tool={tool.position:F3}; PlayerMove={pickup.PlayerMove}.", pickup);
        }
        else if (blockPickup)
        {
            if (block == null)
            {
                if (logNow) Debug.LogWarning($"[PickupMovementDiag] PLAYER_TO_BLOCK SKIPPED: block reference NULL for {pickup.name}#{pickup.GetInstanceID()}.", pickup);
                return;
            }

            player.position = Vector3.MoveTowards(player.position, block.position, pickup.speed * Time.deltaTime);
            if (Vector3.Distance(player.position, block.position) < 0.05f)
            {
                pickup.PlayerMove = false;
                block.position = player.position + facingDirection * pickup.HoldOffsetDistance;
            }
            if (logNow) Debug.Log($"[PickupMovementDiag] PLAYER_TO_BLOCK result; player={player.position:F3}; block={block.position:F3}; PlayerMove={pickup.PlayerMove}.", pickup);
        }
        else if (dropBranch)
        {
            Transform transformToMove = pickup.Tool ? tool : block;
            if (transformToMove == null)
            {
                if (logNow) Debug.LogWarning($"[PickupMovementDiag] DROP SKIPPED: selected transform NULL for {pickup.name}#{pickup.GetInstanceID()}.", pickup);
                return;
            }

            transformToMove.position = Vector3.MoveTowards(transformToMove.position, pickup.CalculatedDropTarget, pickup.speed * Time.deltaTime);
            player.position = Vector3.MoveTowards(player.position, pickup.PlaceBlockTransform.position, pickup.speed * Time.deltaTime);
            if (Vector3.Distance(player.position, pickup.PlaceBlockTransform.position) < 0.1f ||
                Vector3.Distance(transformToMove.position, pickup.CalculatedDropTarget) < 0.1f)
                pickup.ExecuteDropRelease();
        }

        if (logNow)
            Debug.Log($"[PickupMovementDiag] AFTER branch={branch}; object={pickup.name}#{pickup.GetInstanceID()}; self={pickup.transform.position:F3}; " +
                      $"tool={(tool != null ? tool.position.ToString("F3") : "NULL")}; player={player.position:F3}; toolIsSelf={(tool == pickup.transform)}.", pickup);
    }
}