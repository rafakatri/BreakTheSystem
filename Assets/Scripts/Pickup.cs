using UnityEngine;

public class PickupSpinner : MonoBehaviour
{
    public float spinSpeed = 100f;

    void Update()
    {
        transform.Rotate(new Vector3 (0,0,1), spinSpeed * Time.deltaTime);
    }
}


