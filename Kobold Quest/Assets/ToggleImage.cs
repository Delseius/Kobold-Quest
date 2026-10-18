using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleImage : MonoBehaviour
{
    public GameObject playerSpriteImage;
    public GameObject worldSquare;

    // A dedicated tracker variable
    private bool isShown = false;

    void Start()
    {
        // Enforce the default starting state when you press play
        playerSpriteImage.SetActive(isShown);
        worldSquare.SetActive(isShown);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
        {
            // Toggle the tracker
            isShown = !isShown;

            // Apply it strictly to both targets
            playerSpriteImage.SetActive(isShown);
            worldSquare.SetActive(isShown);
        }
    }
}



