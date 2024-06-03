using System;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private Animator anim;

    private bool faceRight = true;

    [SerializeField] private bool onGround;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.5f;
    [SerializeField] private LayerMask Ground;

    public int coins = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        groundCheck = transform.Find("groundCheck");

        realSpeed = walkSpeed;
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
            transform.localScale *= new Vector2(-1, 1);
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
        // Здесь можно добавить логику для отображения количества монет на UI и т.д.
    }

    public void RemoveCoin(int amount)
    {
        coins -= amount;
        // Здесь можно добавить логику для отображения количества монет на UI и т.д.
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
                transform.localScale *= new Vector2(-1, 1);
                faceRight = newFaceRight;
            }
        }
    }
}
