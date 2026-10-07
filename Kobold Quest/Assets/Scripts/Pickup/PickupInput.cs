
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
        GameObject currentObject =
            pickup.gameObject;

        if (currentObject.CompareTag("DropZone"))
        {
            if (pickupobject.heldObject != null)
            {
                UnityEngine.Debug.Log(
                    $"Clicked on drop zone {currentObject.name}! " +
                    $"Telling {pickupobject.heldObject.name} to drop here."
                );

                pickupobject heldScript =
                    pickupobject.heldObject
                    .GetComponent<pickupobject>();

                if (heldScript != null)
                {
                    heldScript.InitiateDropFromBlock(
                        currentObject.transform
                    );
                }
            }

            return;
        }
        // Clicking a block with a pickaxe activates it.
        // It does NOT immediately pick it up.
        /*if (currentObject.CompareTag("Block"))
        {
            if (pickupobject.IsPickaxeHeld())
            {
                pickup.UnlockBlockForPickup();

                UnityEngine.Debug.Log(
                    $"Pickaxe activated " +
                    $"{currentObject.name}. " +
                    "The block can now be picked up."
                );
            }
            else
            {
                UnityEngine.Debug.Log(
                    $"{currentObject.name} is locked. " +
                    "Hold a pickaxe and activate " +
                    "the block first."
                );
            }

            return;
        }*/

        // Clicking a block with the correct tool activates the block.
        // Pickaxe -> stone blocks
        // Shovel  -> dirt blocks
        if (currentObject.CompareTag("Block"))
        {
            if (pickupobject.IsCorrectToolForBlock(currentObject))
            {
                pickup.UnlockBlockForPickup();

                UnityEngine.Debug.Log(
                    $"{pickupobject.GetHeldToolName()} activated " +
                    $"{currentObject.name}. Press C to pick it up."
                );
            }
            else
            {
                UnityEngine.Debug.Log(
                    $"{currentObject.name} cannot be activated with " +
                    $"{pickupobject.GetHeldToolName()}. " +
                    "Use the correct tool."
                );
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

        // E = Drop held object at player's feet
        if (
            pickup.held &&
            !pickup.PlayerMove &&
            Keyboard.current.eKey.wasPressedThisFrame
        )
        {
            pickup.DropAtFeet();
            return;
        }

        // F = Use the correct tool on the block under the mouse.
        // Pickaxe -> stone blocks
        // Shovel  -> dirt blocks
        // This is checked before normal F item use.
        if (
            pickup.held &&
            pickupobject.IsCorrectToolHeld() &&
            Keyboard.current.fKey.wasPressedThisFrame
        )
        {
            // Checks a radius around the player instead of the mouse cursor
            if (TryActivateBlockUnderMouse())
            {
                return;
            }
        }

        // F = Use held item
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

        // C = Pick up the nearest Item or Block
        // around the player.
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
                player =
                    pickupobject.FindPlayerTransform();
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
    private bool TryActivateBlockUnderMouse()
    {
        if (Camera.main == null)
        {
            return false;
        }

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        Vector3 worldPosition =
            Camera.main.ScreenToWorldPoint(
                new Vector3(
                    mousePosition.x,
                    mousePosition.y,
                    -Camera.main.transform.position.z
                )
            );

        Collider2D hit =
            Physics2D.OverlapPoint(
                new Vector2(
                    worldPosition.x,
                    worldPosition.y
                )
            );

        if (hit == null)
        {
            return false;
        }

        pickupobject block =
            hit.GetComponent<pickupobject>();

        if (
            block == null ||
            !block.CompareTag("Block") ||
            !pickupobject.IsCorrectToolForBlock(block.gameObject)
        )
        {
            return false;
        }

        pickupobject.SelectBlock(block);
        block.UnlockBlockForPickup();

        UnityEngine.Debug.Log(
            $"F used {pickupobject.GetHeldToolName()} on " +
            $"{block.gameObject.name}. The block can now be picked up."
        );

        return true;
    }

    /*private bool TryActivateBlockInRadius()
    {
        Transform playerTransform = pickup.GetPlayerTransform;

        if (playerTransform == null)
        {
            Debug.LogError("PlayerTransform is not assigned on the pickupobject script!");
            return false;
        }

        float interactionRadius = 3.0f; // Adjust this number for how close you want the player to stand

        // 1. Find all 2D colliders inside a circle centered on the PLAYER
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(playerTransform.position, interactionRadius);

        pickupobject closestBlock = null;
        float closestDistance = Mathf.Infinity;

        // 2. Find the closest valid block
        foreach (var collider in hitColliders)
        {
            pickupobject foundBlock = collider.GetComponent<pickupobject>();

            if (foundBlock != null)
            {
                float distance = Vector2.Distance(playerTransform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBlock = foundBlock;
                }
            }
        }

        if (closestBlock != null)
        {
            // Because it's public static, you can assign it directly like this!
            pickupobject.selectedBlock = closestBlock;

            closestBlock.UnlockBlockForPickup();
            return true;
        }


        return false;
    }*/








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
            player =
                pickupobject.FindPlayerTransform();
        }

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