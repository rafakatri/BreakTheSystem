using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;
    private float force = 10;
    private float camHalfWidth, leftBoundary, rightBoundary;
    private Camera mainCamera;
    private float damage = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player"); 
        mainCamera = Camera.main;
        camHalfWidth = mainCamera.orthographicSize * mainCamera.aspect; 
    }

    public void SetDirection(Vector3 shootingDirection)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(shootingDirection.x, shootingDirection.y).normalized * force;
    }

    // Update is called once per frame
    void Update()
    {
        leftBoundary = mainCamera.transform.position.x - camHalfWidth;
        rightBoundary = mainCamera.transform.position.x + camHalfWidth;

        if (transform.position.x < leftBoundary || transform.position.x > rightBoundary){
            Destroy(gameObject);
        }
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

    public void SetDamage(float dmg) {
        damage = dmg;
    }

    public float GetDamage() {
        return damage;
    }
}
