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

    // The fill area is the pool's collider box grown by fillRadius on
    // every side (the cyan box drawn when selected).
    public bool IsInRange(Vector3 position)
    {
        Bounds area = GetFillArea();

        return
            position.x >= area.min.x && position.x <= area.max.x &&
            position.y >= area.min.y && position.y <= area.max.y;
    }

    private Bounds GetFillArea()
    {
        Collider2D areaCollider =
            poolCollider != null
                ? poolCollider
                : GetComponent<Collider2D>();

        Bounds area =
            areaCollider != null
                ? areaCollider.bounds
                : new Bounds(transform.position, Vector3.zero);

        area.Expand(new Vector3(fillRadius * 2f, fillRadius * 2f, 0f));

        return area;
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
        Bounds area = GetFillArea();

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(area.center, area.size);
    }
}
