using System.Collections.Generic;
using UnityEngine;

// Attach to the water pool GameObject so buckets can be filled from it.
public class WaterPool : MonoBehaviour
{
    [Header("Water Pool")]
    [SerializeField]
    [Tooltip("How close (in world units) the player must be to fill a bucket.")]
    private float fillRadius = 1.5f;

    private static readonly List<WaterPool> activePools =
        new List<WaterPool>();

    private Collider2D poolCollider;

    private void Awake()
    {
        poolCollider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        activePools.Add(this);
    }

    private void OnDisable()
    {
        activePools.Remove(this);
    }

    /// <summary>
    /// Distance from a point to the edge of the pool.
    /// Uses the pool's Collider2D if it has one, otherwise its position.
    /// </summary>
    public float DistanceTo(Vector3 position)
    {
        if (poolCollider != null)
        {
            Vector2 closestPoint =
                poolCollider.ClosestPoint(position);

            return Vector2.Distance(closestPoint, position);
        }

        return Vector2.Distance(transform.position, position);
    }

    public bool IsInRange(Vector3 position)
    {
        return DistanceTo(position) <= fillRadius;
    }

    /// <summary>
    /// Returns the nearest pool the given position is close enough to use,
    /// or null if there is none.
    /// </summary>
    public static WaterPool FindPoolInRange(Vector3 position)
    {
        WaterPool nearest = null;
        float nearestDistance = Mathf.Infinity;

        foreach (WaterPool pool in activePools)
        {
            if (pool == null || !pool.IsInRange(position))
            {
                continue;
            }

            float distance = pool.DistanceTo(position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = pool;
            }
        }

        return nearest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, fillRadius);
    }
}
