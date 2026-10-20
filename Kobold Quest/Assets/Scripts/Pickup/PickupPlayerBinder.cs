using System.Reflection;
using UnityEngine;

/// <summary>
/// Assigns the active scene Player to a pickup object's private PlayerTransform
/// when a prefab cannot serialize a reference to a scene object.
/// Add this component to reusable pickup/block prefabs.
/// </summary>
[RequireComponent(typeof(pickupobject))]
public class PickupPlayerBinder : MonoBehaviour
{
    private static readonly FieldInfo PlayerTransformField =
        typeof(pickupobject).GetField(
            "PlayerTransform",
            BindingFlags.Instance | BindingFlags.NonPublic
        );

    private pickupobject pickup;

    private void Awake()
    {
        pickup = GetComponent<pickupobject>();
        TryBindPlayer();
    }

    private void Start()
    {
        // Retry after scene objects have initialized, in case Awake ran too early.
        TryBindPlayer();
    }

    private void TryBindPlayer()
    {
        if (pickup == null)
        {
            pickup = GetComponent<pickupobject>();
        }

        if (pickup == null || pickup.PlayerTransformReference != null)
        {
            return;
        }

        if (PlayerTransformField == null)
        {
            Debug.LogError(
                "PickupPlayerBinder could not find pickupobject.PlayerTransform. " +
                "The pickup framework may have changed.",
                this
            );
            return;
        }

        Transform player = pickupobject.FindPlayerTransform();

        if (player == null)
        {
            Debug.LogWarning(
                $"PickupPlayerBinder could not find a scene Player for {name}.",
                this
            );
            return;
        }

        PlayerTransformField.SetValue(pickup, player);
    }
}
