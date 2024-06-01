using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Slider slider, sliderFuria, sliderExtra;

    public void SetHealth(int health) {
        slider.value = health;
        //check if health is more than 3. Oif yes, increase the sliderExtra value and make the sliderExtra visible. Each slider is max 3
        if (health > 3) {
            sliderExtra.value = health - 3;
            sliderExtra.gameObject.SetActive(true);
        } else {
            sliderExtra.gameObject.SetActive(false);
        }
    }

    public void SetHealthMecha(int health) {
        slider.value = health;
    }

    public void SetMaxHealth(int health) {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetFuria(int furia) {
        sliderFuria.value = furia;
    }

    public int GetFuria() {
        return (int)sliderFuria.value;
    }

}
