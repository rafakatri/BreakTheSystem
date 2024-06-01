using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mecha : MonoBehaviour
{
    public float _speedX = 5f;
    private float _speedY = 5f;
    private float _stopRunningThreshold = 0.12f;
    private Vector2 _movement = Vector2.zero;
    private Rigidbody2D _rb;
    private Animator _animator;
    private Renderer _render;
    private bool _isGrounded = true;
    private float _camHalfWidth, _leftBoundary, _rightBoundary, _playerWidth;
    private float _timeSinceLastMovement;
    private Camera _mainCamera;
    public float hp = 15;
    private bool isDead = false;
    public GameObject groundCheck;
    private GroundCheck _gc;
    private Vector3 BulletPosition;
    public GameObject bullet, mechaPickUp;
    public float damage = 5;
    private GameObject playerOld;
    private HUDManager hudManager;
    private GameObject mechaBar;
    private GameObject playerBar, extraPlayerBar;
    private GameObject exit;
    private PlayerInput input;
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private int remainingBulletsPickUpGun;
    private int gunIndex;
    private AudioManager audioManager;
    private bool _isWalkingSoundPlaying = false;

    private void Start()
    {
        exit = GameObject.FindGameObjectWithTag("Exit");
        mechaBar = GameObject.FindGameObjectWithTag("MechaBar");
        playerBar = GameObject.FindGameObjectWithTag("PlayerBar");
        playerBar.SetActive(false);
        extraPlayerBar = GameObject.FindGameObjectWithTag("ExtraPlayerBar");
        if (extraPlayerBar != null) {
            extraPlayerBar.SetActive(false);
        }
        hudManager = GameObject.FindGameObjectWithTag("MechaHUD").GetComponent<HUDManager>();
        hudManager.SetMaxHealth((int) hp);
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        audioManager.PlayMechaEnter();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _render = GetComponent<Renderer>();
        _mainCamera = Camera.main;
        if (_mainCamera != null) _camHalfWidth = _mainCamera.orthographicSize * _mainCamera.aspect;
        _gc = groundCheck.GetComponent<GroundCheck>();
        BulletPosition = new Vector3(2.305133f, -0.03f, 0f);
        _mainCamera.GetComponent<CameraController>().UpdatePlayer();
        input = GameObject.FindGameObjectWithTag("input").GetComponent<PlayerInput>();
        input.actions["Move"].performed += OnMovePerform;
        input.actions["Move"].canceled += OnMoveExit;
        input.actions["Jump"].performed += OnJumpPerform;
        input.actions["Shoot"].performed += OnShoot;
        UpdateAllEnemies();
                
    }

    private bool IsTouchInLeftHalf()
    {
        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed && touch.position.ReadValue().x < Screen.width / 2)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void OnMovePerform(InputAction.CallbackContext context)
    {
        if (IsTouchInLeftHalf())
        {
            if (!isDead)
            {
                _movement = context.ReadValue<Vector2>();
                _timeSinceLastMovement = 0f;
            }
            if (_isGrounded && !isDead && context.ReadValue<Vector2>().y > 0.5f)
            {
                _rb.AddForce(Vector2.up * _speedY, ForceMode2D.Impulse);
            }
        }
    }

    private void OnMoveExit(InputAction.CallbackContext context) {
        if (!IsTouchInLeftHalf())
        {
            _movement = Vector2.zero;
            _animator.SetBool(IsRunning, false);
        }
    }


    private void OnJumpPerform(InputAction.CallbackContext context)
    {
        if (_isGrounded && !isDead && IsTouchInLeftHalf())
        {
            _rb.AddForce(Vector2.up * _speedY, ForceMode2D.Impulse);
        }
    }

    private void OnShoot(InputAction.CallbackContext context) {
        Vector3 globalPos = transform.TransformPoint(BulletPosition);
        audioManager.PlayMechaGunShot();
        Instantiate(bullet, globalPos, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("HpUp")) {
            Destroy(other.gameObject);
            hp += 1;
            hudManager.SetHealthMecha((int) hp);
        }

        else if (other.CompareTag("EnemyBullet")) {
            TakeDamage(other.gameObject.GetComponent<EnemyBullet>().GetDamage());
            Destroy(other.gameObject);
        }
    }

    private void FixedUpdate()
    {
        if(!isDead) {
            _rb.velocity = new Vector2(_movement.x * _speedX, _rb.velocity.y);
            _timeSinceLastMovement += Time.deltaTime;
        }
    }

    private void Update()
    {
        _playerWidth = _render.bounds.size.x;
        _leftBoundary = _mainCamera.transform.position.x - _camHalfWidth + _playerWidth / 2;
        _rightBoundary = _mainCamera.transform.position.x + _camHalfWidth - _playerWidth / 2;
        
        if (_gc.GetIsGrounded())
            _isGrounded = true;
        else
            _isGrounded = false;

        Vector3 playerPos = transform.position;
        playerPos.x = Mathf.Clamp(playerPos.x, _leftBoundary, _rightBoundary);
        transform.position = playerPos;
        //check if the player is moving in x in the ground and play the walking sound
        if (_isGrounded && Math.Abs(_movement.x) > 0 && !_isWalkingSoundPlaying)
        {
            if (!_isWalkingSoundPlaying && _isGrounded)
            {
                audioManager.PlayMechaWalking();
                _isWalkingSoundPlaying = true;
            }
            //if it is already playing, check if it is still grounded
            else if (_isWalkingSoundPlaying && !_isGrounded)
            {
                audioManager.StopMechaWalking();
                _isWalkingSoundPlaying = false;
            }
        }
        if (Math.Abs(_movement.x) > 0 && IsTouchInLeftHalf())
        {
            _animator.SetBool(IsRunning, true);

            if (_movement.x > 0 && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (_movement.x < 0 && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        else if (_timeSinceLastMovement > _stopRunningThreshold)
        {
            _animator.SetBool(IsRunning, false);
            _movement = Vector2.zero;
            // Stop the walking sound if it was playing
            if (_isWalkingSoundPlaying)
            {
                audioManager.StopMechaWalking(); 
                _isWalkingSoundPlaying = false;
            }

        } 
        
        _animator.SetBool(IsJumping, !_isGrounded);

        CheckExitDistance();

    }

    public void Die(){
        _rb.velocity = Vector2.zero;
        isDead = true;
        audioManager.PlayMechaDead();
        _animator.SetTrigger("death");
        input.actions["Move"].performed -= OnMovePerform;
        input.actions["Move"].canceled -= OnMoveExit;
        input.actions["Jump"].performed -= OnJumpPerform;
        input.actions["Shoot"].performed -= OnShoot;
        playerOld.GetComponent<Player>().SubscribeInput();
        playerOld.transform.GetChild(0).GetComponent<Arm>().SubscribeInput();
        playerOld.transform.GetChild(0).GetChild(0).GetComponent<Gun>().SubscribeInput();
        playerOld.SetActive(true);
        mechaBar.SetActive(false);
        playerBar.SetActive(true);
        if (extraPlayerBar != null) {
            extraPlayerBar.SetActive(true);
        }
        playerOld.transform.position = transform.position;
        gameObject.SetActive(false);
        playerOld.GetComponent<Player>().SetGunIndex(gunIndex);
        playerOld.transform.GetChild(0).GetChild(0).GetComponent<Gun>().SetRemainingBullets(remainingBulletsPickUpGun);
        //stop the walking sound if it is playing
        if (_isWalkingSoundPlaying)
        {
            audioManager.StopMechaWalking();
            _isWalkingSoundPlaying = false;
        }
        UpdateAllEnemies();
        Destroy(gameObject);
        _mainCamera.GetComponent<CameraController>().UpdatePlayer();
    }

    public void TakeDamage(float dmg){
        hp -= dmg;
        hudManager.SetHealthMecha((int) hp);
        audioManager.PlayPlayerHurt();
        _animator.SetTrigger("hurt");
        if (hp <= 0 && !isDead)
        {
            Die();
        }
    }

    public bool getIsDead() {
        return isDead;
    }

    public float GetDamage(){
        return damage;
    }

    private void UpdateAllEnemies() {
        
        GameObject[] rangeEnemies = GameObject.FindGameObjectsWithTag("RangeEnemy");
        GameObject[] meleeEnemies = GameObject.FindGameObjectsWithTag("MeleeEnemy");

        foreach (GameObject e in rangeEnemies) {
            
            EnemyShooting  _es = e.GetComponent<EnemyShooting>();
            if (_es != null) {
                _es.UpdatePlayer();
            }
            else {
                e.GetComponent<EnemyShootingDownwards>().UpdatePlayer();
            }

        }

        foreach (GameObject e in meleeEnemies) {
            e.GetComponent<EnemyMelee>().UpdatePlayer();
        }
    }

    public void SetOldPlayer(GameObject player){
        playerOld = player;
        remainingBulletsPickUpGun = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).GetChild(0).GetComponent<Gun>().GetRemainingBullets();
        gunIndex = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().GetGunIndex();

    }

    private void CheckExitDistance() {
    float distanceToExit = Vector3.Distance(transform.position, exit.transform.position);

    if (distanceToExit < 6f) {
        _rb.AddForce(new Vector2(0, -0.1f), ForceMode2D.Impulse);

        if (_isGrounded &&!isDead) {
            Instantiate(mechaPickUp, transform.position, transform.rotation);
            Die();
        }
    }
}

}
