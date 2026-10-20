using System.Reflection;
using UnityEngine;

/// <summary>
/// Lets a picked-up stone block be broken into two smaller Stone material items.
/// Attach this component to the scene's stone_Block object and assign the Stone prefab.
/// This uses the existing IUsable/F-key flow without changing pickup-system scripts.
/// </summary>
public class StoneBlockSplitter : MonoBehaviour, IUsable
{
    // Use UnityEngine.Object because this Unity version serializes the assigned asset as a Prefab,
    // which does not deserialize into a GameObject-typed field.
    [SerializeField] private UnityEngine.Object stoneMaterialPrefab;
    [SerializeField, Min(2)] private int materialYield = 2;
    [SerializeField, Min(0.1f)] private float spawnSeparation = 0.35f;
    [SerializeField] private float spawnYOffset = -0.25f;

    // pickupobject exposes this serialized reference as a getter only. Assign it on
    // spawned outputs so they use the real scene Player rather than a prefab asset.
    private static readonly FieldInfo PlayerTransformField =
        typeof(pickupobject).GetField(
            "PlayerTransform",
            BindingFlags.Instance | BindingFlags.NonPublic
        );

    public bool Use(GameObject user)
    {
        if (user == null)
        {
            Debug.LogWarning("Cannot split stone: the user was not supplied.", this);
            return false;
        }

        if (!CompareTag("Block") ||
            !gameObject.name.ToLowerInvariant().Contains("stone"))
        {
            Debug.LogWarning("StoneBlockSplitter must be attached to a stone Block.", this);
            return false;
        }

        if (pickupobject.heldObject != gameObject)
        {
            Debug.Log("Pick up the stone block before using it.", this);
            return false;
        }

        if (stoneMaterialPrefab == null)
        {
            Debug.LogError("Assign the Stone material prefab on StoneBlockSplitter.", this);
            return false;
        }

        if (PlayerTransformField == null)
        {
            Debug.LogError(
                "Could not link spawned Stone items to the player. " +
                "The pickupobject PlayerTransform field may have changed.",
                this
            );
            return false;
        }

        Transform player = user.transform;
        Vector3 facing = player.localScale.x < 0f ? Vector3.left : Vector3.right;
        Vector3 basePosition = player.position + Vector3.up * spawnYOffset;
        int spawnedCount = 0;

        for (int i = 0; i < materialYield; i++)
        {
            float side = i % 2 == 0 ? 1f : -1f;
            int row = i / 2;
            Vector3 spawnPosition =
                basePosition +
                facing * (side * spawnSeparation * (row + 1)) +
                Vector3.down * (row * spawnSeparation);

            UnityEngine.Object spawnedObject = Instantiate(
                stoneMaterialPrefab,
                spawnPosition,
                Quaternion.identity
            );

            GameObject stone = spawnedObject as GameObject;
            if (stone == null)
            {
                Debug.LogError(
                    "The Stone material reference instantiated as a non-GameObject. " +
                    "Assign the Stone prefab asset in the Inspector.",
                    this
                );
                if (spawnedObject != null)
                {
                    Destroy(spawnedObject);
                }
                continue;
            }

            pickupobject stonePickup = stone.GetComponent<pickupobject>();
            if (stonePickup == null)
            {
                Debug.LogError(
                    "The Stone material prefab needs its existing pickupobject component.",
                    stone
                );
                Destroy(stone);
                continue;
            }

            PlayerTransformField.SetValue(stonePickup, player);
            spawnedCount++;
        }

        if (spawnedCount != materialYield)
        {
            Debug.LogWarning(
                $"Stone splitting spawned {spawnedCount} of {materialYield} materials.",
                this
            );
        }

        if (spawnedCount == 0)
        {
            return false;
        }

        Debug.Log($"Split {gameObject.name} into {spawnedCount} Stone material items.", this);

        // Give UseHeldItem time to clear the held state before destroying the block.
        Destroy(gameObject, 0.05f);
        return true;
    }
}
