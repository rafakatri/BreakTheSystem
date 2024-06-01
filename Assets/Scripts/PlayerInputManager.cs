using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private GameObject player, arm, gun;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        player = GameObject.FindGameObjectWithTag("Player");
        arm = player.transform.GetChild(0).gameObject;
        gun = arm.transform.GetChild(0).gameObject;
    }

    private void OnDestroy()
    {
        try
        {
            if (playerInput!= null)
            {
                if (player!= null) {
                    player.GetComponent<Player>().UnsubscribeInput();
                }
                if (arm!= null) {
                    arm.GetComponent<Arm>().UnsubscribeInput();
                }
                if (gun!= null) {
                    gun.GetComponent<Gun>().UnsubscribeInput();
                }
            }
        }
        catch (NullReferenceException)
        {

        }
    }
}
