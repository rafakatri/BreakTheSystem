using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChangeBGColor : MonoBehaviour
{
    private Color colorOne = Color.green;
    private Color colorTwo = Color.yellow;
    public float intervalSeconds = 1f;

    private Image panelImage;

    private void Start()
    {
        // Get the Image component from the Panel GameObject
        panelImage = GetComponent<Image>();

        // Start the color changing coroutine
        StartCoroutine(ChangeColorEveryFewSeconds());
    }

    private IEnumerator ChangeColorEveryFewSeconds()
    {
        while (true) // Loop indefinitely
        {
            // Switch to color one
            panelImage.color = colorOne;
            // Wait for the specified interval
            yield return new WaitForSeconds(intervalSeconds);
            // Switch to color two
            panelImage.color = colorTwo;
            // Wait for the specified interval
            yield return new WaitForSeconds(intervalSeconds);
        }
    }
}
