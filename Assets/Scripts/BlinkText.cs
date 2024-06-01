using System.Collections;
using UnityEngine;
using TMPro;

public class BlinkText : MonoBehaviour
{
    public TextMeshProUGUI subtextComponent;

    void Start()
    {
        if (subtextComponent == null)
        {
            subtextComponent = GameObject.Find("Canvas/Subtext").GetComponent<TextMeshProUGUI>();
        }
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        while (true)
        {
            // The rest of your code remains the same
            subtextComponent.enabled = false;
            yield return new WaitForSeconds(0.5f);
            subtextComponent.enabled = true;
            yield return new WaitForSeconds(1f);
        }
    }
}
