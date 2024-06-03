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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        groundCheck = transform.Find("groundCheck");

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
        currentHealth = maxHealth; // Устанавливаем текущее здоровье равным максимальному при старте
        coinsTextMeshPro.text = coins.ToString();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            Walk();
            Run();
            Jump();
            SetDirection();
            CheckGround();
        }
        else
        {
            // Обновляем анимацию на основе полученных данных
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
        }
    }

    private void CheckGround()
    {
        onGround = Physics2D.OverlapCircle(groundCheck.position, checkRadius, Ground);
        anim.SetBool("onGround", onGround);
    }

    public void AddCoin(int amount)
    {
        coins += amount;
        UpdateCoinsUI(); // Обновляем UI при изменении количества монет
    }

    public void RemoveCoin(int amount)
    {
        coins -= amount;
        UpdateCoinsUI(); // Обновляем UI при изменении количества монет
    }

    private void UpdateCoinsUI()
    {
        if (coinsTextMeshPro != null)
        {
            coinsTextMeshPro.text = coins.ToString(); // Обновляем текст с количеством монет
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Отправляем данные другим игрокам
            stream.SendNext(moveVector.x);
            stream.SendNext(realSpeed);
            stream.SendNext(onGround);
            stream.SendNext(faceRight);
        }
        else
        {
            // Получаем данные от других игроков
            moveVector.x = (float)stream.ReceiveNext();
            realSpeed = (float)stream.ReceiveNext();
            onGround = (bool)stream.ReceiveNext();
            bool newFaceRight = (bool)stream.ReceiveNext();

            if (newFaceRight != faceRight)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                faceRight = newFaceRight;
            }
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
            currentHealth = maxHealth; // Убедитесь, что текущее здоровье не превышает максимальное
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
