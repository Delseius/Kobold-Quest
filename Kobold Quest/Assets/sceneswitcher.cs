using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneswitcher : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "GameplayScene";

    public void ResumeGame()
    {
        // Restore active game logic processing time
        Time.timeScale = 1f;

        // Return back to your main game loop scene
        SceneManager.LoadScene(0);
    }
}
