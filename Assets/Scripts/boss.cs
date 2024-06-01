using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private string _estado;
    private GameObject _player;
    private Gun _playerGun;
    private Rigidbody2D _rb;
    private Animator _animator;
    public float perception = 10f;
    public float hp = 2f;
    private float _initHp; 
    public int damage = 1;
    public bool isDead;
    public GameObject Minion;
    public float moveSpeed;
    private float _initMoveSpeed;
    private float _moveLockStart = 5f, _moveLock = 5f;
    private float minionSpacing = 1.5f;
    private int numberOfMinions = 1;
    private readonly float _moveSpeedY = 1f;
    private bool _attacked;
    public bool summon = false, stopLaser = false;
    private float _lastAttackTime;
    public float attackCooldown, pursuerCoolDown;
    private float _jumpTimeStart, _pursuerTime;
    private bool _setAttackAnimation = false;
    private CapsuleCollider2D _coll;
    private SpriteRenderer _spr;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int Hurt = Animator.StringToHash("hurt");
    private static readonly int Death = Animator.StringToHash("death");
    private AudioManager _audioManager;
    private string[] estados = {"corpoAcorpo", "distancia", "jump", "summon"};
    private string[] estados_above = {"jump", "summon"};

    // Start is called before the first frame update
    private void Start()
    {
        _initMoveSpeed = moveSpeed;
        _initHp = hp;
        _estado = "waitingPlayer";
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerGun = GameObject.FindGameObjectWithTag("Gun").GetComponent<Gun>();
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _coll = GetComponent<CapsuleCollider2D>();
        _spr = GetComponent<SpriteRenderer>();
        _audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        //Debug.Log(_estado);
        switch (_estado)
        {
            case "waitingPlayer":
                WaitingPlayer();
                return;
            case "setTarget":
                SetTarget();
                return;
            case "corpoAcorpo":
                CorpoACorpo();
                return;
            case "distancia":
                Distancia();
                return;
            case "summon":
                Summon();
                return;
            case "jump":
                Jump();
                return;
            case "getAbovePlayer":
                GetAbovePlayer();
                return;
            case "cair":
                Cair();
                return;
            case "Idle":
                Idle();
                return;
            case "cooldown":
                Cooldown();
                return;
            case "dead":
                Dead();
                return;
        }
    }

    private void WaitingPlayer()
    {
        if (_player is null) return;
        var enemyPosition = new Vector2(transform.position.x, transform.position.y);
        var playerPosition = new Vector2(_player.transform.position.x, _player.transform.position.y);
        var distance = Vector2.Distance(enemyPosition, playerPosition);
        if (distance <= perception)
        {
            _estado = "setTarget";
        }
    }

    private void SetTarget()
    {
        UpdateFacingDirection();
        int objetivo;
        if (GetDistanceY() > 2.6f) {
            Debug.Log("Above: " + GetDistanceY());
            objetivo = Random.Range(0, 2);
            _estado = estados_above[objetivo];
        }
        else {
            objetivo = Random.Range(0, 4);
            _estado = estados[objetivo];
        }
    }

    private void Jump(){
        if (transform.position.y >= 1.38f){
            _estado = "getAbovePlayer";
            _jumpTimeStart = Time.time;
        }
        _rb.velocity = new Vector2(0f, _moveSpeedY);
    }

    private void GetAbovePlayer(){
        if (_rb.velocity.x  < 0.1 && Time.time >= _jumpTimeStart + 1){
            _rb.velocity = Vector2.zero;
            _estado = "cair";
            _pursuerTime = Time.time;
            _moveLock = _moveLockStart;
        }
        _rb.velocity = new Vector2(_moveLock * GetDirectionX() , 0f);
        UpdateFacingDirection();
        if (Time.time >= _jumpTimeStart + 1){
            _moveLock -= _moveLock * 0.1f;
        }
    }

    private void Cair(){
        if (transform.position.y <= 0f){
            _estado = "cooldown";
        }
        _rb.velocity = new Vector2(0f, -_moveSpeedY);
    }

    private void Idle(){
        _attacked = false;
        _rb.velocity = Vector2.zero;
    }

    private void Dead(){
        _attacked = false;
        _rb.velocity = Vector2.zero;
        if (transform.position.y > -0.24f) {
            _rb.velocity = new Vector2(0f, -_moveSpeedY);
        }
    }


    private void CorpoACorpo()
    {
        if (_pursuerTime == 0f){
            _pursuerTime = Time.time;
        }
        MoveTowardsPlayer(0.5f);
        if (Time.time >= _pursuerTime + pursuerCoolDown){
            _estado = "cooldown";
        }
    }

    private void Distancia()
    {
        if (_pursuerTime == 0f){
            _pursuerTime = Time.time;
        }
        
        var dist = Random.Range(1, 5);

        if (GetDistanceX() > dist){
            MoveTowardsPlayer(dist);
        }
        else if (!_setAttackAnimation){
            _animator.SetBool("attack", true);
            _audioManager.PlayBossLaser();
            _setAttackAnimation = true;
        }
        else if (stopLaser) {
            _animator.SetBool("attack", false);
            stopLaser = false;
        }
        if (_spr.sprite.bounds.size != _coll.bounds.size)
        {
            _coll.size = _spr.sprite.bounds.size;
        }

        if (Time.time >= _pursuerTime + pursuerCoolDown){
            _estado = "cooldown";
        }
    }

    private void Summon(){
        _animator.SetTrigger("attack2");
        if (summon){
            _pursuerTime = Time.time;
            InstantiateSummon();
            _estado = "cooldown";
        }
    }

    public void InstantiateSummon(){
        Vector3 pos = new Vector3(-0.6f, -1.77f, 0f);
        pos = transform.TransformPoint(pos);
        if (hp <= _initHp/3){
            numberOfMinions = 3;
        }
        else if (hp <= _initHp/2) {
            numberOfMinions = 2;
        }
        for (int i = 0; i < numberOfMinions; i++) {
            Vector3 minionPosition = pos + Vector3.right * (i * minionSpacing - 0.5f);
            Instantiate(Minion, minionPosition, Quaternion.identity);
        }
        summon = false;
    }

    private void Cooldown(){
        if(_attacked || _pursuerTime != 0f){
            _animator.SetBool("attack", false);
            _lastAttackTime = Time.time;
            _pursuerTime = 0f;
            _setAttackAnimation = false;
            _attacked = false;
        }

        if (_estado == "summon" && Time.time >= _lastAttackTime + pursuerCoolDown) {
            _estado = "setTarget";
        }
        else if (Time.time >= _lastAttackTime + attackCooldown){
            _estado = "setTarget";
        }
        _rb.velocity = Vector2.zero;
    }

    private float GetDistanceX(){
        return Vector2.Distance(new Vector2(transform.position.x, 0), new Vector2(_player.transform.position.x, 0));
    }

    private float GetDistanceY(){
        return Vector2.Distance(new Vector2(0, transform.position.y), new Vector2(0, _player.transform.position.y));
    }

    private float GetDirectionX(){
        var targetDirection = _player.transform.position - transform.position;
        targetDirection.y = 0;
        targetDirection.Normalize();
        return targetDirection.x;
    }


    private void UpdateFacingDirection()
    {
        var directionX = GetDirectionX();

        transform.localScale = directionX switch
        {
            > 0 => new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z),
            < 0 => new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z),
            _ => transform.localScale
        };
    }

    private void MoveTowardsPlayer(float maxDistance)
    {
        if (_player is null) return;

        if (hp <= _initHp/3) {
            moveSpeed = _initMoveSpeed + 2;
        }
        else if (hp <= _initHp/2) {
            moveSpeed = _initMoveSpeed + 1;
        }

        if (GetDistanceX() > maxDistance)
        {
            UpdateFacingDirection();
            transform.Translate(Vector3.right * (GetDirectionX() * moveSpeed * Time.deltaTime));
            _animator.SetBool(IsWalking, true);
        }
        else
        {
            _animator.SetBool(IsWalking, false);
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("bullet")){
            Destroy(other.gameObject);
            TakeDamage(_playerGun.GetDamage());
        } 
        else if (other.gameObject.CompareTag("Player"))
        {
            switch (_estado)
            {
                case "distancia" or "corpoAcorpo" when !_attacked:
                    other.gameObject.GetComponent<Player>().TakeDamage(damage);
                    _attacked = true;
                    _estado = "cooldown";
                    break;
                case "cair" when !_attacked:
                    other.gameObject.GetComponent<Player>().TakeDamage(damage);
                    _attacked = true;
                    break;
            }
        }
    }

    public void TakeDamage(float damageTaken)
    {
        if (!isDead){
            hp -= damageTaken;
            _animator.SetTrigger(Hurt);
        }
        if (hp <= 0 && !isDead)
        {
            Die(); 
        }
    }

    private void Die()
    {
        isDead = true;
        _audioManager.PlayBossDeath();
        _animator.SetTrigger(Death);
        _estado = "dead";
    }
}
