using UnityEngine;

public class EnemyShootingDownwards : MonoBehaviour
{
    public GameObject BulletManual;
    public Transform BulletPosition;
    private GameObject player;
    private Gun player_gun;
    private float timer;
    public float shootingDistance = 1f;
    public float moveSpeed = 3f;
    public float damage = 1f;
    public float hp = 2;
    private bool isDead = false;
    private bool isGrounded = false;
    public float attackCooldown = 1.5f;
    public int numberOfShots = 1;
    private float bulletSpacing = 1;
    private float lastAttackTime = 0f;
    public float perception = 5;
    private Rigidbody2D rb;
    private Animator animator;
    private Mecha mecha;
    private bool isMecha = false;
    private BoxCollider2D col;
    private HUDManager hudManager;
    private AudioManager audioManager;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player_gun = GameObject.FindGameObjectWithTag("Gun").GetComponent<Gun>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        hudManager = GameObject.FindGameObjectWithTag("PlayerHUD").GetComponent<HUDManager>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("bullet")) {
            Destroy(other.gameObject);
            TakeDamage(player_gun.GetDamage());
        }
        else if (other.CompareTag("MechaBullet")) {
            TakeDamage(mecha.GetDamage());
        }
        else if (other.CompareTag("Ground")){
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (!isDead)
        {
            if (IsPlayerInRange() && !ShouldWait())
            {
                Shoot();
            }
            else if(IsPlayerNear() && !ShouldWait()){
                MoveTowardsPlayer();
            }
            else 
            {
                animator.SetBool("isWalking", false);
            }
        }
        else if (isGrounded){
            Destroy(gameObject, 1f);
        }
    }

    bool ShouldWait()
    {
        if (player != null)
        {
            return Time.time < lastAttackTime + attackCooldown;
        }
        return false;
    }

    void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector3 targetDirection = player.transform.position - transform.position;
            targetDirection.y = 0;
            targetDirection.Normalize();

            if (targetDirection.x > 0){
                transform.Translate(targetDirection * moveSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (targetDirection.x < 0){
                transform.Translate(-targetDirection * moveSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, 180, 0);

            }
            animator.SetBool("isWalking", true);
        }
    }

    bool IsPlayerInRange()
    {
        if (player != null)
        {
            Vector2 enemyPosition = new Vector2(transform.position.x, 0f);
            Vector2 playerPosition = new Vector2(player.transform.position.x, 0f);
            float distance = Vector2.Distance(enemyPosition, playerPosition);
            return distance <= shootingDistance;
        }
        return false;
    }

    bool IsPlayerNear()
    {
        if (player != null)
        {
            Vector2 enemyPosition = new Vector2(transform.position.x, 0f);
            Vector2 playerPosition = new Vector2(player.transform.position.x, 0f);
            float distance = Vector2.Distance(enemyPosition, playerPosition);
            return distance <= perception; 
        }
        return false;
    }

    private void Shoot()
    {
        Vector3 targetDirection = player.transform.position - transform.position;
        targetDirection.x = 0;
        targetDirection.Normalize();

        if (targetDirection.x > 0){
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (targetDirection.x < 0){
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        animator.SetTrigger("attack");
        lastAttackTime = Time.time;
        for (int i = 0; i < numberOfShots; i++) {
            Vector3 bulletPosition = BulletPosition.position + Vector3.up * (i * bulletSpacing - 0.5f);
            GameObject bullet = Instantiate(BulletManual, bulletPosition, Quaternion.identity);
            EnemyBullet enemyBullet = bullet.GetComponent<EnemyBullet>();
            if (enemyBullet != null){
                enemyBullet.SetDirection(targetDirection);
                enemyBullet.SetDamage(damage);
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

    private void Die() {
        isDead = true;
        audioManager.PlayEnemyDead();
        animator.SetTrigger("death");
        rb.bodyType = RigidbodyType2D.Dynamic;
        col.isTrigger = true;
        hudManager.SetFuria(hudManager.GetFuria() + 1);
    }

    public void TakeDamage(float dmg) {
        if (!isDead) {
            hp -= dmg;
            animator.SetTrigger("hurt");
        }
        if (hp <= 0 && !isDead){
            Die();
        }
    }

    public void UpdatePlayer() {
        player = GameObject.FindGameObjectWithTag("Player");
        isMecha = !isMecha;
        if (isMecha){
            mecha = player.GetComponent<Mecha>();
        }
    }

}