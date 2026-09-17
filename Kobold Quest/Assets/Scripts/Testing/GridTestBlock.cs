using UnityEngine;

public class TestGridBlock : GridObject
{
    protected override void Start()
    {
        base.Start();

        Debug.Log(
            gameObject.name +
            " is in grid cell " +
            GridPosition
        );

        GridSpace.Instance.DebugCell(GridPosition);
    }
}