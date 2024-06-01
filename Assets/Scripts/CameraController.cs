using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject player;
    private Vector3 offset, newPosition, target;
    public bool isFreezed = false;
    private bool wasFreezed = false;
    public float maxY;
    private float step;
    public float movementSpeed = 0.01f;
    public float acceleration = 0.03f;
    public float deadEnd_left;
    public float deadEnd_right;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        offset = transform.position - player.transform.position;
    }

    public void UpdatePlayer(){
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!isFreezed){
            if(wasFreezed && player.transform.position.x < (transform.position.x - offset.x)){
                newPosition = transform.position;
                newPosition.y = Mathf.Clamp(newPosition.y, 0, maxY);
                transform.position = newPosition;
                return;
            }
            else if (wasFreezed && player.transform.position.x > (transform.position.x - offset.x)) {
                step = step = Mathf.Lerp(step, movementSpeed, acceleration * Time.deltaTime);
                target = player.transform.position + offset;
                target.y = Mathf.Clamp(target.y, 0, maxY);
                transform.position = Vector3.Lerp(transform.position, target, step);

                if (Vector3.Distance(transform.position, target) < 0.1f){
                    wasFreezed = false;
                }
            }
            else {
                wasFreezed = false;
                newPosition = player.transform.position + offset;
                newPosition.y = Mathf.Clamp(newPosition.y, 0, maxY);
                if (newPosition.x >= deadEnd_left && newPosition.x <= deadEnd_right) {
                    transform.position = newPosition;
                }
            }
        }
        else {
            wasFreezed = true;
        }
    }
}