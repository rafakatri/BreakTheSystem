using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevel : MonoBehaviour
{
    // Start is called before the first frame update
    public string nextScreen;
   
    public void Exit(){
        if (SceneManager.GetActiveScene().name == "level_3"){
            SceneManager.LoadScene(nextScreen);
        } else {
            AdsManager.Instance.interstitialAds.ShowInterstitialAd(nextScreen);
        }
        //SceneManager.LoadScene(nextScreen); // Agora é no InterstitialAds.cs
    }
}
