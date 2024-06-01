using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenuSelector : MonoBehaviour
{
    public TextMeshProUGUI[] menuOptions;
    public InputActionAsset inputActions;

    public void ShowControls()
    {
        SceneManager.LoadScene("ControlScreen", LoadSceneMode.Additive);
    }

    public void ShowSoundSettings()
    {
        SceneManager.LoadScene("SoundScreen", LoadSceneMode.Additive);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1; // Resume the game
        SceneManager.UnloadSceneAsync("PauseMenu");
    }

    public void QuitSound()
    {
        SceneManager.UnloadSceneAsync("SoundScreen");
    }

    public void RestartLevel()
    {
        // Ensure the game is not paused
        Time.timeScale = 1;

        // Directly reload the current scene
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void QuitToMainMenu()
    {
        // Ensure the game is not paused
        Time.timeScale = 1;        
        SceneManager.LoadScene("StartScreen");
    }
}
