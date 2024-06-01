using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    private AudioSource buttonPress;
    private Image flashImage;
    
    void Start()
    {
        buttonPress = GameObject.Find("ButtonPress").GetComponent<AudioSource>();
        flashImage = GameObject.Find("FlashImage").GetComponent<Image>();
    }

    void OnMenu()
    {
        StartCoroutine(RestartGame());
    }

    IEnumerator RestartGame()
    {
        buttonPress.Play();

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

        SceneManager.LoadScene("StartScreen");
    }
}
