using UnityEngine;
using UnityEngine.InputSystem;

public class TestGridBlock : GridObject
{
    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SnapToGrid();
        }
    }
}