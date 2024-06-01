using UnityEngine;
using UnityEngine.InputSystem;

public class Arm : MonoBehaviour
{
    private GameObject player;
    private PlayerInput input;
    private Player playerScript;
    private Vector2 dir;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<Player>();
        input = GameObject.FindGameObjectWithTag("input").GetComponent<PlayerInput>();
        input.actions["Aim"].started += OnAimEnter;
        input.actions["Aim"].canceled += OnAimExit;
        input.actions["AimUp"].started += OnAimUpEnter;
        input.actions["AimUp"].canceled += OnAimUpExit;
        input.actions["AimDown"].started += OnAimDownEnter;
        input.actions["AimDown"].canceled += OnAimDownExit;
        input.actions["FullAim"].performed += OnFullAim;
        input.actions["FullAim"].canceled += OnFullAimExit;
    }

    //Update to put the arm in the right position when IsTouchInRightHalf is false
    void Update()
    {
        if (!IsTouchInRightHalf())
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void FlipPlayer() {
        player.transform.localScale = new Vector3(player.transform.localScale.x * -1, player.transform.localScale.y, player.transform.localScale.z);
    }

    private bool IsTouchInRightHalf()
    {
        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed && touch.position.ReadValue().x >= Screen.width / 2)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void OnFullAim(InputAction.CallbackContext context) {
        dir = context.ReadValue<Vector2>();

        float targetRotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (player.transform.localScale.x < 0) {
            targetRotation += 180;
        }

        transform.rotation = Quaternion.Euler(0, 0, targetRotation);

    }

    void OnFullAimExit(InputAction.CallbackContext context) {
        if (!IsTouchInRightHalf())
        {
        dir = context.ReadValue<Vector2>();
        transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        
    }

    void OnAimEnter(InputAction.CallbackContext context)
    {
        AimAtAngle(30f);
    }

    void OnAimUpEnter(InputAction.CallbackContext context)
    {
        AimAtAngle(45f);
    }

    void OnAimDownEnter(InputAction.CallbackContext context)
    {
        AimAtAngle(-45f);
    }

    void OnAimExit(InputAction.CallbackContext context)
    {
        if (!IsTouchInRightHalf())
        {
        AimAtAngle(-30f);
        }
    }

    void OnAimUpExit(InputAction.CallbackContext context)
    {
        if (!IsTouchInRightHalf())
        {
        AimAtAngle(-45f);
        }
    }

    void OnAimDownExit(InputAction.CallbackContext context)
    {
        if (!IsTouchInRightHalf())
        {
        AimAtAngle(45f);
        }
    }

    void AimAtAngle(float angle)
    {
        if(!playerScript.getIsDead()){
            transform.Rotate(new Vector3(0f,0f,angle));
        }
    }

    public void Deactivate(){
        gameObject.SetActive(false);
    }

    public void UnsubscribeInput()
    {
        input.actions["Aim"].started -= OnAimEnter;
        input.actions["Aim"].canceled -= OnAimExit;
        input.actions["AimUp"].started -= OnAimUpEnter;
        input.actions["AimUp"].canceled -= OnAimUpExit;
        input.actions["AimDown"].started -= OnAimDownEnter;
        input.actions["AimDown"].canceled -= OnAimDownExit;
        input.actions["FullAim"].performed -= OnFullAim;
        input.actions["FullAim"].canceled -= OnFullAimExit;
    }

    private void OnDestroy()
    {
        if (input != null) {
            input.actions["Aim"].started -= OnAimEnter;
            input.actions["Aim"].canceled -= OnAimExit;
            input.actions["AimUp"].started -= OnAimUpEnter;
            input.actions["AimUp"].canceled -= OnAimUpExit;
            input.actions["AimDown"].started -= OnAimDownEnter;
            input.actions["AimDown"].canceled -= OnAimDownExit;
            input.actions["FullAim"].performed -= OnFullAim;
            input.actions["FullAim"].canceled -= OnFullAimExit;
        }
    }

    public void SubscribeInput()
    {
        input.actions["Aim"].started += OnAimEnter;
        input.actions["Aim"].canceled += OnAimExit;
        input.actions["AimUp"].started += OnAimUpEnter;
        input.actions["AimUp"].canceled += OnAimUpExit;
        input.actions["AimDown"].started += OnAimDownEnter;
        input.actions["AimDown"].canceled += OnAimDownExit;
        input.actions["FullAim"].performed += OnFullAim;
        input.actions["FullAim"].canceled += OnFullAimExit;
    }
}
