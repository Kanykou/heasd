using System.Collections;
using UnityEngine;

public class DashSkill : MonoBehaviour
{
    public float dashDistance = 5f; // ระยะพุ่ง
    public float dashSpeed = 20f; // ความเร็วในการพุ่ง
    public float cooldownTime = 1f; // เวลาคูลดาวน์
    private float cooldownTimer = 0f; // ตัวจับเวลาคูลดาวน์
    private bool isDashing = false; // สถานะการพุ่ง

    private Vector2 dashDirection; // ทิศทางการพุ่ง
    private Rigidbody2D rb; // Rigidbody ของตัวละคร

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // ดึง Rigidbody2D ของ Player
    }

    void Update()
    {
        // ลดค่า Cooldown Timer
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            return; // หยุดการทำงานถ้ายังไม่พ้นคูลดาวน์
        }

        // ตรวจจับทิศทางจากปุ่มลูกศร
        dashDirection = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) dashDirection += Vector2.up; // บน
        if (Input.GetKey(KeyCode.S)) dashDirection += Vector2.down; // ล่าง
        if (Input.GetKey(KeyCode.A)) dashDirection += Vector2.left; // ซ้าย
        if (Input.GetKey(KeyCode.D)) dashDirection += Vector2.right; // ขวา

        // หากมีการกดทิศทาง และกดปุ่ม Dash (เช่น F)
        if (dashDirection != Vector2.zero && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        cooldownTimer = cooldownTime; // เริ่มคูลดาวน์

        // Normalize ทิศทางเพื่อให้มีความยาวเป็น 1
        dashDirection = dashDirection.normalized;

        // คำนวณเวลาที่ใช้ในการพุ่ง
        float dashTime = dashDistance / dashSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < dashTime)
        {
            rb.velocity = dashDirection * dashSpeed; // เคลื่อนที่ในทิศทางที่เลือก
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero; // หยุดตัวละครหลัง Dash
        isDashing = false;
    }
}
