using UnityEngine;

public class TestGridObject : GridObject
{
    protected override void Start()
    {
        base.Start();

        Debug.Log(
            gameObject.name +
            " is occupying grid cell " +
            GridPosition
        );

        transform.position =
            GetGridWorldPosition();
    }
}