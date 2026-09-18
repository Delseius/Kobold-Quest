using UnityEngine;

public class PickupDropSystem
{
    private pickupobject pickup;

    public PickupDropSystem(pickupobject pickup)
    {
        this.pickup = pickup;
    }

    public void InitiateDropFromBlock(Transform targetDestination)
    {
        if (targetDestination == null)
        {
            return;
        }

        UnityEngine.Debug.Log(
            $"Drop state activated. Target destination block: " +
            $"{targetDestination.name}"
        );

        pickup.PlaceBlockTransform =
            targetDestination;

        pickup.CalculatedDropTarget =
            new Vector3(
                targetDestination.position.x,
                targetDestination.position.y +
                pickup.VerticalDropOffset,
                targetDestination.position.z
            );

        if (pickup.ObjectCollider != null)
        {
            pickup.ObjectCollider.enabled = true;
            pickup.ObjectCollider.isTrigger = true;
        }

        pickup.release = false;
        pickup.Pressed = false;
        pickup.held = false;

        pickup.Tool = false;

        pickup.block =
            pickup.gameObject.CompareTag("Block");

        pickup.drop = true;
        pickup.PlayerMove = false;
    }

    public void DropAtFeet()
    {
        UnityEngine.Debug.Log(
            $"E Pressed! Dropping " +
            $"{pickup.gameObject.name} at player's feet."
        );

        if (pickup.PlayerTransformReference == null)
        {
            return;
        }

        pickup.CalculatedDropTarget =
            pickup.PlayerTransformReference.position;

        if (pickup.ObjectSpriteRenderer != null)
        {
            pickup.ObjectSpriteRenderer.sortingOrder = 5;
        }

        ExecuteDropRelease();
    }

    public void HandleTriggerEnter(Collider2D other)
    {
        if (
            pickup.drop &&
            other.transform ==
            pickup.PlaceBlockTransform
        )
        {
            ExecuteDropRelease();
        }
    }

    public void HandleCollisionEnter(Collision2D collision)
    {
        if (
            pickup.drop &&
            collision.transform ==
            pickup.PlaceBlockTransform
        )
        {
            ExecuteDropRelease();
        }
    }

    public void ExecuteDropRelease()
    {
        UnityEngine.Debug.Log(
            $"{pickup.gameObject.name} snapped perfectly " +
            $"to drop targets."
        );

        pickup.transform.SetParent(null);

        pickup.transform.rotation =
            Quaternion.identity;

        pickup.transform.position =
            pickup.CalculatedDropTarget;

        if (
            pickup.ObjectSpriteRenderer != null &&
            pickup.PlaceBlockTransform != null
        )
        {
            SpriteRenderer blockRenderer =
                pickup.PlaceBlockTransform
                    .GetComponent<SpriteRenderer>();

            if (blockRenderer != null)
            {
                pickup.ObjectSpriteRenderer.sortingOrder =
                    blockRenderer.sortingOrder + 1;
            }
            else
            {
                pickup.ObjectSpriteRenderer.sortingOrder = 5;
            }
        }

        if (pickup.ObjectBody != null)
        {
            pickup.ObjectBody.linearVelocity =
                Vector2.zero;

            pickup.ObjectBody.angularVelocity = 0f;

            pickup.ObjectBody.bodyType =
                RigidbodyType2D.Static;
        }

        if (pickup.ObjectCollider != null)
        {
            pickup.ObjectCollider.enabled = true;
            pickup.ObjectCollider.isTrigger = false;
        }

        pickupobject.heldObject = null;

        pickup.PlaceBlockTransform = null;

        pickup.drop = false;
        pickup.release = false;
        pickup.held = false;
        pickup.Tool = false;
        pickup.block = false;
        pickup.PlayerMove = false;
    }
}