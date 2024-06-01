using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuActivator : MonoBehaviour
{
    public InputActionAsset inputActions; // Attach your input actions asset here
    private InputAction pauseAction;
    private InputActionMap playerActionMap;
    private InputActionMap uiActionMap;

    public void PauseToggle()
    {
        if (Time.timeScale == 0)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0; // Pause the game
        SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
    }

    private void ResumeGame()
    {
        Time.timeScale = 1; // Resume the game
        SceneManager.UnloadSceneAsync("PauseMenu");
    }
}
