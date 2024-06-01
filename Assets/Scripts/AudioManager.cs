using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioClip[] musicClips;
    private AudioClip _lastClip;
    public AudioClip[] sfxClips;
    private Image flashImage;
    public InputActionAsset inputActions;
    private InputAction anyKeyAction;
    public AudioSource walkingSfxSource; // New AudioSource for walking sound effect
    private string lastScreen;

    public void setLastScreen(string screen)
    {
        lastScreen = screen;
    } 

    void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("AudioManager").Length > 1) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        musicSource.loop = true; // Ensure music loops
        musicSource.pitch = 1.0f; // Reset pitch to normal
        sfxSource.loop = false; // Ensure sound effects don't loop
        sfxSource.pitch = 1.0f; // Reset pitch to normal

        // Get the pause menu action map
        anyKeyAction = inputActions.FindActionMap("PauseMenu").FindAction("AnyKey");

        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        anyKeyAction.performed += OnAnyKeyPressed;
        anyKeyAction.Enable();

        // Try finding the flash image, but don't throw an error if it's not found
        flashImage = GameObject.Find("FlashImage")?.GetComponent<Image>();
        _lastClip = musicClips[1];
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "StartScreen":
                PlayMusic(musicClips[0]);
                break;
            case "Level1":
                PlayMusic(musicClips[1]);
                _lastClip = musicClips[1];
                break;
            case "level_2":
                PlayMusic(musicClips[2]);
                _lastClip = musicClips[2];
                break;
            case "level_3":
                PlayMusic(musicClips[3]);
                _lastClip = musicClips[3];
                break;
            case "PauseMenu":
                PlayMusic(musicClips[4]);
                break;
            case "GameOver":
                PlayGameOverSequence();
                break;
            case "VictoryScreen":
                PlayVictorySequence();
                break;
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        switch (scene.name)
        {
            case "PauseMenu":
                PlayMusic(_lastClip);
                break;
        }
    }

    void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    void PlayGameOverSequence()
    {
        musicSource.Stop();
        sfxSource.PlayOneShot(sfxClips[7]);
    }

    void PlayVictorySequence()
    {
        PlayMusic(sfxClips[8]);
    }

    private void OnAnyKeyPressed(InputAction.CallbackContext context)
    {
        if (SceneManager.GetActiveScene().name == "VictoryScreen")
        {
            LoadScene("StartScreen");
        }
        else if (SceneManager.GetActiveScene().name == "GameOver")
        {
            LoadScene(lastScreen);
        }
        else if (SceneManager.GetActiveScene().name == "StartScreen")
        {
            // Don't destroy audio Listener
            DontDestroyOnLoad(GameObject.Find("AudioListener"));
            LoadScene("ControlScreen");
        }
        else if (SceneManager.GetActiveScene().name == "ControlScreen")
        {
            LoadScene("Level1");
            Destroy(GameObject.Find("AudioListener"));
        }
    }

    void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneWithEffects(sceneName));
    }

    IEnumerator LoadSceneWithEffects(string sceneName)
    {
        // Play a sound effect
        sfxSource.PlayOneShot(sfxClips[6]);

        if (flashImage != null)
        {
            // Flash in
            while (flashImage.color.a < 0.5f)
            {
                Color color = flashImage.color;
                color.a += Time.deltaTime * 5.0f;
                flashImage.color = color;
                yield return null;
            }

            // Flash out
            while (flashImage.color.a > 0.0f)
            {
                Color color = flashImage.color;
                color.a -= Time.deltaTime * 5.0f;
                flashImage.color = color;
                yield return null;
            }
        }

        // Load the scene
        SceneManager.LoadScene(sceneName);
    }

    // Play RegularGunShot sound effect
    public void PlayRegularGunShot()
    {
        sfxSource.PlayOneShot(sfxClips[3]);
    }

    // Play SpecialGunShot sound effect
    public void PlaySpecialGunShot()
    {
        sfxSource.PlayOneShot(sfxClips[4]);
    }

    // Play MechaGunShot sound effect
    public void PlayMechaGunShot()
    {
        sfxSource.PlayOneShot(sfxClips[2]);
    }

    // Play MechaEnter sound effect
    public void PlayMechaEnter()
    {
        sfxSource.PlayOneShot(sfxClips[0]);
    }

    // Play Mecha Walking sound effect
    public void PlayMechaWalking()
    {
        if (!walkingSfxSource.isPlaying) {
            walkingSfxSource.clip = sfxClips[1]; // Assuming index 1 is the walking sound
            walkingSfxSource.loop = true; // Make sure the walking sound loops
            walkingSfxSource.pitch = 2f;
            walkingSfxSource.Play();
        }
    }

    public void StopMechaWalking()
    {
        walkingSfxSource.Stop();
    }

    // Player is hurt
    public void PlayPlayerHurt()
    {
        sfxSource.PlayOneShot(sfxClips[9]);
    }

    // Enemy is dead
    public void PlayEnemyDead()
    {
        sfxSource.PlayOneShot(sfxClips[10]);
    }

    // Mecha is dead
    public void PlayMechaDead()
    {
        sfxSource.PlayOneShot(sfxClips[11]);
    }

    // Player gets machine gun
    public void PlayMachineGunPickup()
    {
        sfxSource.PlayOneShot(sfxClips[12]);
    }

    public void PlayHPPickup()
    {
        sfxSource.PlayOneShot(sfxClips[13]);
    }

    public void PlayExplosion()
    {
        sfxSource.PlayOneShot(sfxClips[14]);
    }

    public void PlayMinionDead()
    {
        sfxSource.PlayOneShot(sfxClips[15]);
    }

    public void PlayBossLaser()
    {
        sfxSource.PlayOneShot(sfxClips[16]);
    }

    public void PlayBossDeath()
    {
        sfxSource.PlayOneShot(sfxClips[17]);
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (anyKeyAction != null) {
            anyKeyAction.Disable();
        }
    }

    void Update()
    {
        // Subscribe to the changes SFXVolume and MusicVolume
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.2f);
        sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 0.2f);
    }

}
