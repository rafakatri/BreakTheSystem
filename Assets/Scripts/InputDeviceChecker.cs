using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDeviceChecker : MonoBehaviour
{
    public GameObject keyboardPanel;
    public GameObject controllerPanel;

    void Update()
    {
        if (Gamepad.current != null)
        {
            controllerPanel.SetActive(true);
            keyboardPanel.SetActive(false);
        }
        else if (Keyboard.current != null)
        {
            controllerPanel.SetActive(false);
            keyboardPanel.SetActive(true);
        }
    }
}
