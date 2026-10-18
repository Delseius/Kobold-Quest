using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleImage : MonoBehaviour
{
    public GameObject playerSpriteImage;
    public GameObject worldSquare;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
        {
            // Read the current state, then invert it
            bool nextState = !playerSpriteImage.activeSelf;

            // Explicitly force both objects to match the exact same true/false state
            playerSpriteImage.SetActive(nextState);
            worldSquare.SetActive(nextState);
        }
    }
}


