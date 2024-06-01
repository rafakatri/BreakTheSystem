using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    private GameObject player;
    private Gun player_gun;
    private Rigidbody2D rb;
    public float moveSpeed = 3f;
    public float perception = 10f;
    public int damage = 1;
    public float attackCooldown = 2f; // Cooldown between attacks
    private float lastAttackTime = 0f;
    public float hp = 2f;
    private bool isDead = false;
    private bool isGrounded= false;
    private Animator animator;
    private Mecha mecha;
    private bool isMecha = false;
    private HUDManager hudManager;
    private Collider2D _collider2D;
    private AudioManager audioManager;

    void Start()
    {
        _collider2D = GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        player_gun = GameObject.FindGameObjectWithTag("Gun").GetComponent<Gun>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        hudManager = GameObject.FindGameObjectWithTag("PlayerHUD").GetComponent<HUDManager>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    void Update()
    {
        if (!isDead)
        {
            if (IsPlayerNear() && !ShouldWait())
            {
                MoveTowardsPlayer();
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }
        else if (isGrounded){
            rb.bodyType = RigidbodyType2D.Static;
            _collider2D.enabled = false;
            Destroy(gameObject, 1f);
        }
    }

    void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector3 targetDirection = player.transform.position - transform.position;
            targetDirection.y = 0;
            targetDirection.Normalize();
            //Calculate the X distance between the player and the enemy horizontally
            float distance = Vector2.Distance(new Vector2(transform.position.x, 0), new Vector2(player.transform.position.x, 0));
            if (distance > 0.5f)
            {
                if (targetDirection.x > 0){
                    transform.Translate(targetDirection * moveSpeed * Time.deltaTime);
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                else if (targetDirection.x < 0){
                    transform.Translate(targetDirection * moveSpeed * Time.deltaTime);
                    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

                }
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }
    }

    bool ShouldWait()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            return distance <= 0.1f || Time.time < lastAttackTime + attackCooldown;
        }
        return false;
    }

    bool IsPlayerNear()
    {
        if (player != null)
        {
            Vector2 enemyPosition = new Vector2(transform.position.x, transform.position.y);
            Vector2 playerPosition = new Vector2(player.transform.position.x, player.transform.position.y);
            float distance = Vector2.Distance(enemyPosition, playerPosition);
            return distance <=  perception;
        }
        return false;
    }

    public void Attack()
    {
        if (player != null)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                animator.SetBool("isWalking", false);
                if (!isMecha){
                    player.GetComponent<Player>().TakeDamage(damage);
                }
                else {
                    player.GetComponent<Mecha>().TakeDamage(damage);
                }
                 animator.SetTrigger("attack");
                lastAttackTime = Time.time;
            }
        }
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
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform")) {
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Player")) {
            Attack();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

    public void TakeDamage(float damageTaken)
    {
        if (!isDead) {
            hp -= damageTaken;
            animator.SetTrigger("hurt");
        }
        if (hp <= 0 && !isDead)
        {
            Die(); 
        }
    }

    void Die()
    {
        isDead = true;
        audioManager.PlayEnemyDead();
        animator.SetTrigger("death");
        hudManager.SetFuria(hudManager.GetFuria() + 1);
    }

    public void UpdatePlayer() {
        player = GameObject.FindGameObjectWithTag("Player");
        isMecha = !isMecha;
        if (isMecha){
            mecha = player.GetComponent<Mecha>();
        }
    }

}
