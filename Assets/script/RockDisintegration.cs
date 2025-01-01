using UnityEngine;

public class RockDisintegration : MonoBehaviour
{
    [Header("Floating Settings")]
    public float amplitude = 0.5f;
    public float frequency = 1f;

    [Header("Disintegration Settings")]
    public float disintegrationTime = 3f;
    public float respawnTime = 5f;

    private Vector3 startPos;
    private float baseY;
    private bool isDisintegrating = false;
    private bool isHidden = false;
    private float disintegrationTimer = 0f;
    private float respawnTimer = 0f;
    private Collider2D rockCollider;

    void Start()
    {
        startPos = transform.position;
        baseY = startPos.y;
        rockCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (!isDisintegrating && !isHidden)
        {
            // การลอย
            float yOffset = amplitude * Mathf.Sin(frequency * Time.time);
            transform.position = new Vector3(startPos.x, baseY + yOffset, startPos.z);
        }
        else if (isDisintegrating)
        {
            // นับเวลาการสลายตัว
            disintegrationTimer += Time.deltaTime;

            if (disintegrationTimer >= disintegrationTime)
            {
                HideRock();
            }
        }
        else if (isHidden)
        {
            // นับเวลา respawn
            respawnTimer += Time.deltaTime;

            if (respawnTimer >= respawnTime)
            {
                RespawnRock();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDisintegrating && !isHidden && collision.gameObject.CompareTag("Player"))
        {
            StartDisintegration();
        }
    }

    private void StartDisintegration()
    {
        isDisintegrating = true;
        disintegrationTimer = 0f;
        rockCollider.enabled = false;  // ปิด collider ทันทีที่เริ่มสลายตัว
    }

    private void HideRock()
    {
        isDisintegrating = false;
        isHidden = true;
        respawnTimer = 0f;
    }

    private void RespawnRock()
    {
        transform.position = startPos;
        isHidden = false;
        rockCollider.enabled = true;  // เปิด collider อีกครั้ง
    }

    public void ResetRock()
    {
        transform.position = startPos;
        isDisintegrating = false;
        isHidden = false;
        disintegrationTimer = 0f;
        respawnTimer = 0f;
        rockCollider.enabled = true;
    }
}