using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentUI : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Set the names or indices of the scenes where the UI should not be visible
        string[] hideInScenes = { "StartScreen", "GameOver" };

        if (Array.Exists(hideInScenes, element => element == scene.name))
        {
            Destroy(this.gameObject);
        }
    }
}
