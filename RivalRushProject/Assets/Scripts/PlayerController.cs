using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float realSpeed;

    [SerializeField] private float jumpForce = 7f;

    private int jumpCount = 0;
    [SerializeField] private int maxJumpValue = 2;

    private float horizontalInput;
    private Vector2 moveVector;

    private Rigidbody2D rb;
    private PhotonView PV;

    [SerializeField] private Animator anim;

    private bool faceRight = true;

    [SerializeField] private bool onGround;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.5f;
    [SerializeField] private LayerMask Ground;

    public int coins = 0;

    public int maxHealth = 100;
    public int currentHealth;

    private PlayerManager playerManager;

    [SerializeField] private GameObject UI;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private TMP_Text coinsTextMeshPro;
    [SerializeField] private TMP_Text ammoTextMeshPro; // Добавлено для отображения патронов

    
    
    [SerializeField] private ShootingArm shootingArm;

    private bool isInAmmoPickupZone = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        groundCheck = transform.Find("groundCheck");
        shootingArm = GetComponentInChildren<ShootingArm>();
        
        realSpeed = walkSpeed;

        PV = GetComponent<PhotonView>();

        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    private void Start()
    {
        if (!PV.IsMine)
        {
            Destroy(UI);
        }
        currentHealth = maxHealth; 
        shootingArm.currentAmmo = shootingArm.maxAmmo; 
        UpdateUI();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            /*if (Input.GetKeyDown(KeyCode.Escape))
            {
                RoomManager.Instance.LeaveRoom();
            }*/
            Walk();
            Run();
            Jump();
            SetDirection();
            CheckGround();
            CheckForAmmoPurchase(); // Проверка нажатия клавиши "E" для покупки патронов
        }
        else
        {
            anim.SetFloat("moveX", Mathf.Abs(moveVector.x));
            anim.SetBool("run", realSpeed == runSpeed);
            anim.SetBool("onGround", onGround);
        }

        if (transform.position.y < -15f)
        {
            Die();
        }
    }

    private void Walk()
    {
        moveVector.x = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveVector.x * realSpeed, rb.velocity.y);

        anim.SetFloat("moveX", Mathf.Abs(moveVector.x));
    }

    private void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift) && onGround)
        {
            anim.SetBool("run", true);
            realSpeed = runSpeed;
        }
        else
        {
            anim.SetBool("run", false);
            realSpeed = walkSpeed;
        }
    }

    private void Jump()
    {
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && (onGround || (++jumpCount < maxJumpValue)))
        {
            rb.AddForce(Vector2.up * jumpForce);
        }

        if (onGround) { jumpCount = 0; }
    }

    private void SetDirection()
    {
        if ((moveVector.x > 0 && !faceRight) || (moveVector.x < 0 && faceRight))
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            faceRight = !faceRight;

            // Если у игрока есть дочерний Canvas, нужно развернуть его обратно
            Transform canvasTransform = transform.Find("CanvasName"); // Название Canvas как в иерархии Unity
            if (canvasTransform != null)
            {
                canvasTransform.localScale = new Vector3(canvasTransform.localScale.x * -1,canvasTransform.localScale.y, canvasTransform.localScale.z);
            }
        }
    }


    private void CheckGround()
    {
        onGround = Physics2D.OverlapCircle(groundCheck.position, checkRadius, Ground);
        anim.SetBool("onGround", onGround);
    }

    private void CheckForAmmoPurchase()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (IsInAmmoPickupZone() && coins >= 1 && shootingArm.currentAmmo < shootingArm.maxAmmo)
            {
                photonView.RPC("PurchaseAmmo", RpcTarget.All, 1); // Покупаем 1 патрон при нажатии "E" в зоне пополнения
            }
            else
            {
                Debug.Log("Недостаточно монет или патроны уже максимальные!");
            }
        }
    }

    private bool IsInAmmoPickupZone()
    {
        return isInAmmoPickupZone;
    }

    public void SetInAmmoPickupZone(bool value)
    {
        isInAmmoPickupZone = value;
    }

    public void AddCoin(int amount)
    {
        coins += amount;
        UpdateCoinsUI();
    }

    public void RemoveCoin(int amount)
    {
        coins -= amount;
        UpdateCoinsUI();
    }

    public void AddAmmo(int amount)
    {
        shootingArm.currentAmmo = Mathf.Min(shootingArm.maxAmmo, shootingArm.currentAmmo + amount);
        UpdateAmmoUI();
        RefillShootingArmAmmo(amount); // Обновляем патроны в ShootingArm
    }

    [PunRPC]
    public void PurchaseAmmo(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            AddAmmo(amount);
            UpdateCoinsUI();
        }
    }

    private void UpdateCoinsUI()
    {
        if (coinsTextMeshPro != null)
        {
            coinsTextMeshPro.text = coins.ToString();
        }
    }

    private void UpdateAmmoUI()
    {
        if (ammoTextMeshPro != null)
        {
            ammoTextMeshPro.text = shootingArm.currentAmmo.ToString();
        }
    }

    public void UpdateUI()
    {
        UpdateCoinsUI();
        UpdateAmmoUI();
    }

    private void RefillShootingArmAmmo(int amount)
    {
        ShootingArm shootingArm = GetComponentInChildren<ShootingArm>();
        if (shootingArm != null)
        {
            shootingArm.RefillAmmo(amount);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(moveVector.x);
            stream.SendNext(realSpeed);
            stream.SendNext(onGround);
            stream.SendNext(faceRight);
            stream.SendNext(shootingArm.currentAmmo);
            stream.SendNext(coins);
        }
        else
        {
            moveVector.x = (float)stream.ReceiveNext();
            realSpeed = (float)stream.ReceiveNext();
            onGround = (bool)stream.ReceiveNext();
            bool newFaceRight = (bool)stream.ReceiveNext();
            shootingArm.currentAmmo = (int)stream.ReceiveNext();
            coins = (int)stream.ReceiveNext();

            if (newFaceRight != faceRight)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                faceRight = newFaceRight;
            }
            UpdateAmmoUI();
            UpdateCoinsUI();
        }
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = (float)currentHealth / maxHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log("Healed! Current Health: " + currentHealth);
    }

    void Die()
    {
        if (PV.IsMine)
        {
            playerManager.CallDie(PV.ViewID);
        }
    }
}
