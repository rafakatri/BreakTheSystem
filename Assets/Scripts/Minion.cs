using MBT;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Transform player;
    private Animator _animator;
    private Gun player_gun;
    public float hp = 2;
    public int damage = 1;
    public float speed = 1;
    public float attackCooldown = 1f;
    private float lastAttackTime = 0f;
    private bool isDead = false;
    private bool _attacked = false;
    private HUDManager hudManager;
    private AudioManager audioManager;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        player_gun = GameObject.FindGameObjectWithTag("Gun").GetComponent<Gun>();
        
        _animator = GetComponent<Animator>();
        hudManager = GameObject.FindGameObjectWithTag("PlayerHUD").GetComponent<HUDManager>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    void Update()
    {
        if (!isDead && player is not null){
            MoveTowardsPlayer();
        }
        if (_attacked){
            Cooldown();
        }

    }


    private void MoveTowardsPlayer(){
        // Calculate direction towards the player
        Vector2 direction = (player.position - transform.position).normalized;
        if (direction.x < 0) {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (direction.x > 0){
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("bullet")) {
            Destroy(other.gameObject);
            TakeDamage(player_gun.GetDamage());
        }
    }

    public void TakeDamage(float damageTaken)
    {
        if (!isDead){
            hp -= damageTaken;
            _animator.SetTrigger("hurt");
        }
        if (hp <= 0 && !isDead)
        {
            Die();
            hudManager.SetFuria(hudManager.GetFuria() + 1); 
        }
    }

    private void Cooldown(){
        if (Time.time >= lastAttackTime + attackCooldown) {
            _attacked = false;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !_attacked)
        {
            lastAttackTime = Time.time;
            other.gameObject.GetComponent<Player>().TakeDamage(damage);
            _attacked = true;
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        audioManager.PlayMinionDead();
        _animator.SetTrigger("death");
        Destroy(gameObject,0.4f);
    }
}
