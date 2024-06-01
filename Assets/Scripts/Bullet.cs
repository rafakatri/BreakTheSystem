using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb, playerRb;
    private Vector3 direction;
    private float force = 500;
    private float camHalfWidth, leftBoundary, rightBoundary;
    private Camera mainCamera;

    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerRb = player.GetComponent<Rigidbody2D>();
        direction = transform.position - player.transform.position;
        rb = GetComponent<Rigidbody2D>();
        if (player.GetComponent<Mecha>() == null){
            direction.y -= 0.11f;
        }
        rb.AddForce(direction.normalized * force);
        mainCamera = Camera.main;
        camHalfWidth = mainCamera.orthographicSize * mainCamera.aspect; 
    }

     private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        leftBoundary = mainCamera.transform.position.x - camHalfWidth;
        rightBoundary = mainCamera.transform.position.x + camHalfWidth;

        if (transform.position.x < leftBoundary || transform.position.x > rightBoundary){
            Destroy(gameObject);
        }
    }
}
