using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public GameObject bullet, bulletMetralhadora;
    private Vector3 localPos, globalPos;
    private PlayerInput input;
    private bool isShooting = false;
    private GameObject player, machineGunUIimg, machineGunUIText, defaltGunUIimg, defaltGunUIText;
    private Player playerScript;
    private float initialFireRate = 0.1f; // Store the initial fire rate
    private float initialDamage = 1; // Store the initial damage
    public float fireRate = 0.1f; 
    private float damage = 1;
    private Coroutine shootingCoroutine;
    private int remainingBulletsPickUpGun = 50; 
    private AudioManager audioManager;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<Player>();
        input = GameObject.FindGameObjectWithTag("input").GetComponent<PlayerInput>();
        input.actions["Shoot"].started += OnShootStart;
        input.actions["Shoot"].canceled += OnShootEnd;
        localPos = new Vector3(0.47f, 0.03f, 0f);
        // Set initial values
        initialFireRate = fireRate;
        initialDamage = damage;
        machineGunUIimg = GameObject.FindGameObjectWithTag("MachineGunUIimg");
        machineGunUIimg.SetActive(false);
        machineGunUIText = GameObject.FindGameObjectWithTag("MachineGunUIText");
        machineGunUIText.SetActive(false);
        defaltGunUIimg = GameObject.FindGameObjectWithTag("GunUIimg");
        defaltGunUIText = GameObject.FindGameObjectWithTag("GunUIText");
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    void Update()
    {
        if (playerScript.GetGunIndex() == 1)
        {
            machineGunUIimg.SetActive(true);
            machineGunUIText.SetActive(true);
            machineGunUIText.GetComponent<TMPro.TextMeshProUGUI>().text = remainingBulletsPickUpGun.ToString();
            defaltGunUIimg.SetActive(false);
            defaltGunUIText.SetActive(false);
        }
    }

    void OnShootStart(InputAction.CallbackContext context) 
    {
        isShooting = true;
        if (shootingCoroutine == null && !playerScript.getIsDead())
        {
            shootingCoroutine = StartCoroutine(ShootCoroutine());
        }
    }

    void OnShootEnd(InputAction.CallbackContext context) 
    {
        isShooting = false;
    }

    IEnumerator ShootCoroutine()
    {
        while (isShooting && (playerScript.GetGunIndex() == 0 || remainingBulletsPickUpGun > 0))
        {
            Shoot();
            if (playerScript.GetGunIndex() != 0){
                remainingBulletsPickUpGun--;
            }
            yield return new WaitForSeconds(fireRate);
        }
        shootingCoroutine = null;

        if (remainingBulletsPickUpGun <= 0 && playerScript.GetGunIndex() != 0)
        {
            playerScript.SetGunIndex(0);
            machineGunUIimg.SetActive(false);
            machineGunUIText.SetActive(false);
            defaltGunUIimg.SetActive(true);
            defaltGunUIText.SetActive(true);
            fireRate = initialFireRate;
            damage = initialDamage;
        }
    }

    void Shoot() 
    {
        globalPos = transform.TransformPoint(localPos);
        //check the gun index to instantiate the correct bullet: 0 = default gun, 1 = machine gun
        if (playerScript.GetGunIndex() == 0){
            audioManager.PlayRegularGunShot();
            Instantiate(bullet, globalPos, Quaternion.identity);
        }else{
            Instantiate(bulletMetralhadora, globalPos, Quaternion.identity);
            audioManager.PlaySpecialGunShot();
        }
    }

    public void SetFireRate(float rate) 
    {
        fireRate = rate;
    }

    public void SetDamage(float dmg) {
        damage = dmg;
    }

    public float GetDamage() {
        return damage;
    }

    public int GetRemainingBullets() {
        return remainingBulletsPickUpGun;
    }

    public void SetRemainingBullets(int bullets) {
        remainingBulletsPickUpGun = bullets;
    }
    public void UnsubscribeInput()
    {
        input.actions["Shoot"].started -= OnShootStart;
        input.actions["Shoot"].canceled -= OnShootEnd;
    }

    private void OnDestroy()
    {
        if (input != null){
            input.actions["Shoot"].started -= OnShootStart;
            input.actions["Shoot"].canceled -= OnShootEnd;
        }
    }
    public void SubscribeInput()
    {
        input.actions["Shoot"].started += OnShootStart;
        input.actions["Shoot"].canceled += OnShootEnd;
    }

}
