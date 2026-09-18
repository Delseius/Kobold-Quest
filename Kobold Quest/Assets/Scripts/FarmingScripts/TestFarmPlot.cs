using UnityEngine;
using UnityEngine.InputSystem;

public class TestFarmPlot : MonoBehaviour
{
    [SerializeField]
    private FarmPlot farmPlot;

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            farmPlot.Plant();
        }
    }
}