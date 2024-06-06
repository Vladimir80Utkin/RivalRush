using Photon.Pun;
using UnityEngine;

public class ShootingArm : MonoBehaviourPun
{
    [SerializeField] private float offset;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shotPoint;
    public int maxAmmo = 30;
    public int currentAmmo;
    [SerializeField] private float timeBetweenShots = 0.5f;
    private float shotTimer = 0f;
    public float bulletSpeed = 10f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerController playerController;
    private void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
        currentAmmo = maxAmmo;
        if (photonView.IsMine)
        {
            Transform parent = transform.parent;
            while (parent != null)
            {
                Camera cam = parent.GetComponent<Camera>();
                if (cam != null)
                { playerCamera = cam;
                    break;
                }
                parent = parent.parent;
            }

            if (playerCamera == null)
            {
                Debug.LogError("Player Camera not found on player!");
            }
        }
    }
    private void Update()
    {
        if (!photonView.IsMine)
            return;

        HandleAiming();
        HandleShooting();
    }
    private void HandleAiming()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Player Camera not assigned!");
            return;
        }

        Vector3 difference = playerCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);
    }
    private void HandleShooting()
    {
        if (currentAmmo <= 0)
            return;
        if (Time.time >= shotTimer && Input.GetMouseButton(0))
        {
            photonView.RPC("Shoot", RpcTarget.All, shotPoint.position, transform.rotation);
            shotTimer = Time.time + timeBetweenShots;
            currentAmmo--;
            playerController.UpdateUI();
        }
    }
    [PunRPC]
    private void Shoot(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = Instantiate(bulletPrefab, position, rotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            Vector3 mousePosition = playerCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = (mousePosition - position).normalized;
            bulletRb.velocity = direction * bulletSpeed;
        }
    }
    public void RefillAmmo(int amount)
    {
        currentAmmo = Mathf.Min(maxAmmo, currentAmmo + amount);
    }
}
