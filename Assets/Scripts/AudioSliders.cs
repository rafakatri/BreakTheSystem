using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioSliders : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public TextMeshProUGUI musicLabel;
    public TextMeshProUGUI sfxLabel;

    void Start()
    {
        // Set slider values from PlayerPrefs or a default value
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.2f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.2f);

        // Initialize selected slider index
        PlayerPrefs.SetInt("AudioConfigSelected", 0); // Default to music slider
        PlayerPrefs.Save();
        SetSliderColor(0);

        // Add listeners to slider events
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void SetSliderColor(int selectedSlider)
    {
        musicLabel.color = selectedSlider == 0 ? Color.red : Color.white;
        sfxLabel.color = selectedSlider == 1 ? Color.red : Color.white;
    }

    public void Update()
    {
        // If the AudioConfigSelected changes, update the slider colors, red (active) and white (inactive)
        int selectedSlider = PlayerPrefs.GetInt("AudioConfigSelected");
        SetSliderColor(selectedSlider);

        // Subscribe to the changes SFXVolume and MusicVolume
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        Debug.Log("Music Volume: " + musicSlider.value);
        Debug.Log("SFX Volume: " + sfxSlider.value);
    }
}
