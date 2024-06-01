using System;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject BulletManual;
    public Transform BulletPosition;
    private GameObject player;
    private Gun player_gun;
    private float timer;
    public float shootingDistance = 10f;
    public float moveSpeed = 3f;
    public float damage;
    public float hp = 2;
    public bool isDownSide = false;
    public bool moveOnPlatform = false;
    public float perception = 10;
    private bool isDead = false;
    private bool isGrounded = false;
    public float attackCooldown = 2f;
    public int numberOfShots = 1;
    private float bulletSpacing = 1;
    private float lastAttackTime = 0f;
    private Rigidbody2D rb;
    private Animator animator;
    private Mecha mecha;
    private bool isMecha = false;
    private HUDManager hudManager;
    private AudioManager audioManager;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player_gun = GameObject.FindGameObjectWithTag("Gun").GetComponent<Gun>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
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
    }

     void Update()
    {
        if (!isDead)
        {
            if (IsPlayerNear())
            {
                if (IsPlayerInRange() && Time.time >= lastAttackTime + attackCooldown)
                {
                    animator.SetBool("isWalking", false);
                    Shoot();
                    lastAttackTime = Time.time;
                }
                else if (!IsPlayerInRange() && (transform.position.y < -2f || moveOnPlatform)) {
                    MoveTowardsPlayer();
                }
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }
        else if (isGrounded){
            rb.bodyType = RigidbodyType2D.Static;
            GetComponent<Collider2D>().enabled = false;
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
            float distance = Vector3.Distance(transform.position, player.transform.position);
            return distance <= shootingDistance;
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
            return distance <= perception; 
        }
        return false;
    }

    private void Shoot()
    {
        Vector3 targetDirection = player.transform.position - transform.position;
        float dir = 1;
        if (!isDownSide){
            targetDirection.y = 0;
        }
        else {
            dir = Math.Sign(targetDirection.x);
            targetDirection.x = dir * 0.70710678118f; 
            targetDirection.y = -0.70710678118f;
        }
        targetDirection.Normalize();

        if (targetDirection.x > 0){
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (targetDirection.x < 0){
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        animator.SetTrigger("attack");
        for (int i = 0; i < numberOfShots; i++) {
            Quaternion bulletRotation = Quaternion.identity;
            if (isDownSide)
            {
                bulletRotation = Quaternion.Euler(0, 0, 45 * dir * -1);
            }
            Vector3 bulletPosition = BulletPosition.position + Vector3.right * (i * bulletSpacing - 0.5f);
            GameObject bullet = Instantiate(BulletManual, bulletPosition, bulletRotation);
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