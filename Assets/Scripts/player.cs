using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player : MonoBehaviour
{
    private float _speedX = 5f;
    private float _speedY = 7.5f;
    public Collider2D standingCollider;
    public Collider2D crouchingCollider;
    private float _stopRunningThreshold = 0.12f;
    private Vector2 _movement = Vector2.zero;
    private Rigidbody2D _rb;
    private Animator _animator, _gunAnimator;
    public GameObject gun, mecha;
    private Renderer _render;
    private bool _isGrounded = true;
    private bool _isSitdown = false;
    private float _camHalfWidth, _leftBoundary, _rightBoundary, _playerWidth;
    private float _timeSinceLastMovement;
    private Camera _mainCamera;
    private int hp = 3;
    private int grenadeCount;
    private int bulletsRemaining;
    private bool isDead = false;
    public GameObject groundCheck;
    private GroundCheck _gc;
    private static readonly int GunIndex = Animator.StringToHash("GunIndex");
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private Gun gunScript;
    private PlayerInput input;
    private HUDManager hudManager;
    private GameObject mechaBar, extraHealthBar;
    private bool isOnExit = false;
    private ExitLevel exit;
    Gamepad gamepad;

    //Granade variables
    public float fieldImpact;
    public float force;
    public LayerMask layerToHit;
    public GameObject explosionEffect;
    private AudioManager audioManager;
    private Vector3 accelerationDir;
    private AndroidJavaObject plugin;

    private void Start()
    {
        _renderers = GetComponentsInChildren<SpriteRenderer>();
        _arm1 = gameObject.GetComponentInChildren<Arm>();
        _arm = gameObject.GetComponentInChildren<Arm>();
        exit = GameObject.FindGameObjectWithTag("Exit").GetComponent<ExitLevel>();
        mechaBar = GameObject.FindGameObjectWithTag("MechaBar");
        mechaBar.SetActive(false);
        extraHealthBar = GameObject.FindGameObjectWithTag("ExtraPlayerBar");
        extraHealthBar.SetActive(false);
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        hudManager = GameObject.FindGameObjectWithTag("PlayerHUD").GetComponent<HUDManager>();
        hudManager.SetMaxHealth(hp);
        _rb = GetComponent<Rigidbody2D>();
        input = GameObject.FindGameObjectWithTag("input").GetComponent<PlayerInput>();
        input.actions["Sitdown"].started += OnSitdownEnter;
        input.actions["Sitdown"].canceled += OnSitdownExit;
        input.actions["Move"].performed += OnMovePerform;
        input.actions["Move"].canceled += OnMoveExit;
        input.actions["Jump"].performed += OnJumpPerform;
        input.actions["Enter"].performed += OnEnterPerform;
        input.actions["Furia"].performed += OnGrenade;
        _animator = GetComponent<Animator>();
        _render = GetComponent<Renderer>();
        _mainCamera = Camera.main;
        if (_mainCamera != null) _camHalfWidth = _mainCamera.orthographicSize * _mainCamera.aspect;
        _gunAnimator = gun.GetComponent<Animator>();
        _gc = groundCheck.GetComponent<GroundCheck>();
        gunScript = gun.GetComponent<Gun>();
        standingCollider.enabled = true;
        crouchingCollider.enabled = false;

        #if UNITY_ANDROID
                plugin = new AndroidJavaClass("jp.kshoji.unity.sensor.UnitySensorPlugin").CallStatic<AndroidJavaObject>("getInstance");
        #endif

        #if UNITY_ANDROID
        if (plugin != null) {
                plugin.Call("startSensorListening", "accelerometer");
        }
        #endif
    }

    void OnApplicationQuit ()
    {
        #if UNITY_ANDROID
                if (plugin != null) {
                    plugin.Call("terminate");
                    plugin = null;
                }
        #endif
	}

    private void OnMovePerform(InputAction.CallbackContext context)
    {
        if (!isDead && !_isSitdown)
        {
            _movement = context.ReadValue<Vector2>();
            _timeSinceLastMovement = 0f;
        }
        if (_isGrounded && !isDead && !_isSitdown && !isOnExit && context.ReadValue<Vector2>().y > 0.5f)
        {
            _rb.AddForce(Vector2.up * _speedY, ForceMode2D.Impulse);
        }
    }

    private void OnMoveExit(InputAction.CallbackContext context)
    {
        if (!IsTouchInLeftHalf())
        {
            _movement = Vector2.zero;
        }
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


    private void OnJumpPerform(InputAction.CallbackContext context)
    {
        if (_isGrounded && !isDead && !_isSitdown)
        {
            _rb.AddForce(Vector2.up * _speedY, ForceMode2D.Impulse);
        }
    }

    private void OnEnterPerform(InputAction.CallbackContext context)
    {
        if (isOnExit){
            StartCoroutine(LoadSceneAfterExit());
        }
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("MechaPickUp");

        foreach (GameObject obj in objectsWithTag)
        {
            SpriteRenderer objRenderer = obj.GetComponent<SpriteRenderer>();
            if (objRenderer != null)
            {
                if (IsPlayerInFront(objRenderer))
                {
                    GameObject prefab = Instantiate(mecha, obj.transform.position, obj.transform.rotation);
                    UnsubscribeInput();
                    gunScript.UnsubscribeInput();
                    gun.transform.parent.GetComponent<Arm>().UnsubscribeInput();
                    prefab.GetComponent<Mecha>().SetOldPlayer(gameObject);
                    Destroy(obj);
                    mechaBar.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnGrenade(InputAction.CallbackContext context)
    {
        if (!isDead && hudManager.GetFuria() >= 10)
        {
            Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, fieldImpact, layerToHit);
            foreach (Collider2D obj in objects)
            {
                Handheld.Vibrate();
                if (obj.CompareTag("MeleeEnemy"))
                {
                    if (obj.TryGetComponent(out EnemyMelee enemyMelee))
                    {
                        enemyMelee.TakeDamage(5);
                    }
                    else if (obj.TryGetComponent(out Boss boss))
                    {
                        boss.TakeDamage(5);
                    }
                }
                else if (obj.CompareTag("RangeEnemy"))
                {
                    if (obj.TryGetComponent(out EnemyShooting enemyRange))
                    {
                        enemyRange.TakeDamage(5);
                    }
                    else if (obj.TryGetComponent(out EnemyShootingDownwards enemyRangeDownwards))
                    {
                        enemyRangeDownwards.TakeDamage(5);
                    }
                }
                else if (obj.CompareTag("Minion"))
                {
                    if (obj.TryGetComponent(out EnemyMovement minion))
                    {
                        minion.TakeDamage(5);
                    }
                }
            }
            audioManager.PlayExplosion();
            GameObject ExplosionEffectIns = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(ExplosionEffectIns, 0.8f);
            hudManager.SetFuria(0);
        }
    }
    
    private bool IsPlayerInFront(SpriteRenderer objRenderer)
    {
        Bounds objBounds = objRenderer.bounds;

        Vector3 playerPos = transform.position;
        Vector3 objPos = objBounds.center;

        if (Mathf.Abs(playerPos.y - objPos.y) < objBounds.extents.y)
        {
            if (Mathf.Abs(playerPos.x - objPos.x) < objBounds.extents.x)
            {
                return true;
            }
        }

        return false;
    }

    private void OnSitdownEnter(InputAction.CallbackContext context)
    {
        if (_isGrounded && !isDead && !_animator.GetBool("isSitdown"))
        {
            _isSitdown = true;
            standingCollider.enabled = false;
            crouchingCollider.enabled = true;
            _animator.SetBool("isSitdown", true);
        }
    }

    private void OnSitdownExit(InputAction.CallbackContext context)
    {
        if (_isGrounded && !isDead && _isSitdown && _animator.GetBool("isSitdown") && IsTouchInLeftHalf())
        {
            _isSitdown = false;
            standingCollider.enabled = true;
            crouchingCollider.enabled = false;
            _animator.SetBool("isSitdown", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("GunPickup")) {
            audioManager.PlayMachineGunPickup();
            Destroy(other.gameObject);
            if (GetGunIndex() == 1)
            {
               gunScript.SetRemainingBullets(gunScript.GetRemainingBullets() + 50);
            }else{
                gunScript.SetRemainingBullets(50);
            } 
            _gunAnimator.SetInteger(GunIndex, 1);
            gunScript.SetFireRate(0.2f);
            gunScript.SetDamage(1);
        }

        else if (other.CompareTag("HpUp")) {
            audioManager.PlayHPPickup();
            Destroy(other.gameObject);
            hp += 1;
            hudManager.SetHealth(hp);
        }

        else if (other.CompareTag("EnemyBullet")) {
            TakeDamage((int)other.gameObject.GetComponent<EnemyBullet>().GetDamage());
            Destroy(other.gameObject);
        }

        else if (other.CompareTag("Exit")) {
            isOnExit = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Exit")) {
            isOnExit = false;
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

        accelerationDir = Input.acceleration;
        Debug.Log(accelerationDir.sqrMagnitude.ToString());
        if (accelerationDir.sqrMagnitude > 5f)
        {
            OnGrenade(new InputAction.CallbackContext());
        }

        if (_gc.GetIsGrounded() && _rb.velocity.y < 0.1f)
            _isGrounded = true;
        else
            _isGrounded = false;

        Vector3 playerPos = transform.position;
        playerPos.x = Mathf.Clamp(playerPos.x, _leftBoundary, _rightBoundary);
        transform.position = playerPos;

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
        else if (IsTouchInLeftHalf() && _isSitdown)
        {
            _animator.SetBool(IsRunning, false);
            _animator.SetBool(IsJumping, false);
            _animator.SetBool("isSitdown", true);
        }
        else if (_timeSinceLastMovement > _stopRunningThreshold)
        {
            _animator.SetBool(IsRunning, false);
            _movement = Vector2.zero;
        }

        _animator.SetBool(IsJumping, !_isGrounded);

    }

    public void Die(){
        _rb.velocity = Vector2.zero;
        isDead = true;
        _animator.SetTrigger("death");
        gameObject.GetComponentInChildren<Arm>().Deactivate();
        StartCoroutine(LoadSceneAfterDeath());
    }

    private Coroutine blinkCoroutine;  // Coroutine reference
    private Arm _arm;
    private Arm _arm1;
    private SpriteRenderer[] _renderers;

    public void TakeDamage(int dmg){
        hp -= dmg;
        hudManager.SetHealth(hp);
        Rumble(1f, 0.5f);
        Handheld.Vibrate();
        audioManager.PlayPlayerHurt();
        if (blinkCoroutine != null)  // Stop the previous coroutine if it's still running
        {
            StopCoroutine(blinkCoroutine);
            // Ensure colors are reset even if the coroutine is stopped early
            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (var renderer in renderers)
            {
                renderer.color = Color.white;
            }
        }
        blinkCoroutine = StartCoroutine(Blink(Color.red, 0.1f, 5));  // Start a new blinking coroutine
        if (hp <= 0 && !isDead)
        {
            hudManager.SetHealth(0);
            Die();
        }
    }

    private IEnumerator Blink(Color blinkColor, float duration, int count)
    {
        Color[] originalColors = new Color[_renderers.Length];

        // Store original colors
        for (int i = 0; i < _renderers.Length; i++)
        {
            originalColors[i] = _renderers[i].color;
        }

        // Blink effect
        for (int i = 0; i < count; i++)
        {
            foreach (var renderer in _renderers)
            {
                renderer.color = blinkColor;  // Change to blink color
            }
            yield return new WaitForSeconds(duration);

            for (int j = 0; j < _renderers.Length; j++)
            {
                _renderers[j].color = originalColors[j];  // Revert to original color
            }
            yield return new WaitForSeconds(duration);
        }
        // Ensure colors are reset even if the coroutine is stopped early
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].color = originalColors[i];
        }
    }

    public bool getIsDead() {
        return isDead;
    }

    private IEnumerator LoadSceneAfterDeath() {
        // Wait for 1 second after the player is marked as dead
        yield return new WaitForSeconds(1f);
        Destroy(_arm);
        audioManager.setLastScreen(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("GameOver");
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator LoadSceneAfterExit() {
        _arm1.UnsubscribeInput();
        gameObject.GetComponentInChildren<Arm>().Deactivate();
        _animator.SetTrigger("happy");
        yield return new WaitForSeconds(1f);
        exit.Exit();
    }

    public int GetGunIndex() 
    {
        return _gunAnimator.GetInteger(GunIndex);
    }

    public void SetGunIndex(int index) 
    {
        _gunAnimator.SetInteger(GunIndex, index);
    }
    public void Rumble(float intensity, float duration)
    {
        gamepad = input.GetDevice<Gamepad>();
        if (gamepad == null) return;
        gamepad.SetMotorSpeeds(intensity, duration);
        Invoke("StopRumble", duration);
    }

    public void StopRumble()
    {
        gamepad?.SetMotorSpeeds(0, 0);
    }

    public void UnsubscribeInput(){
        if (input != null){
            input.actions["Sitdown"].started -= OnSitdownEnter;
            input.actions["Sitdown"].canceled -= OnSitdownExit;
            input.actions["Move"].performed -= OnMovePerform;
            input.actions["Move"].canceled -= OnMoveExit;
            input.actions["Jump"].performed -= OnJumpPerform;
            input.actions["Enter"].performed -= OnEnterPerform;
            input.actions["Furia"].performed -= OnGrenade;
        }
    }

    private void OnDestroy(){
        if (input != null) {
            input.actions["Sitdown"].started -= OnSitdownEnter;
            input.actions["Sitdown"].canceled -= OnSitdownExit;
            input.actions["Move"].performed -= OnMovePerform;
            input.actions["Move"].canceled -= OnMoveExit;
            input.actions["Jump"].performed -= OnJumpPerform;
            input.actions["Enter"].performed -= OnEnterPerform;
            input.actions["Furia"].performed -= OnGrenade;
        }
    }

    public void SubscribeInput(){
        if (input != null){
            input.actions["Sitdown"].started += OnSitdownEnter;
            input.actions["Sitdown"].canceled += OnSitdownExit;
            input.actions["Move"].performed += OnMovePerform;
            input.actions["Move"].canceled += OnMoveExit;
            input.actions["Jump"].performed += OnJumpPerform;
            input.actions["Enter"].performed += OnEnterPerform;
            input.actions["Furia"].performed += OnGrenade;
        }
    }

}
