using UnityEngine;
using System.Collections;

public class FloatingDisintegratingRock : MonoBehaviour
{
    [Header("Floating Settings")]
    public float amplitude = 0.5f;    // ความสูงของการลอย
    public float frequency = 1f;      // ความเร็วของการลอย

    [Header("Disintegration Settings")]
    public float disintegrationTime = 3f;  // เวลาที่หินจะหายไป
    public float respawnTime = 5f;         // เวลาที่หินจะกลับมา

    private Vector3 startPos;              // ตำแหน่งเริ่มต้น
    private bool isDisintegrating = false; // สถานะการสลายตัว
    private bool isFloating = true;        // สถานะการลอย

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (isFloating)
        {
            // คำนวณการเคลื่อนที่แบบลอย
            float yOffset = amplitude * Mathf.Sin(frequency * Time.time);
            transform.position = startPos + new Vector3(0, yOffset, 0);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // ตรวจสอบการชนกับ Player
        if (!isDisintegrating && collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DisintegrateAndRespawn());
        }
    }

    private IEnumerator DisintegrateAndRespawn()
    {
        isDisintegrating = true;
        isFloating = false;  // หยุดการลอย

        // เรียกใช้แอนิเมชันถ้ามี
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Disintegrate");
        }

        yield return new WaitForSeconds(disintegrationTime);
        
        // ซ่อนวัตถุ
        gameObject.SetActive(false);

        yield return new WaitForSeconds(respawnTime);

        // รีเซ็ตตำแหน่งและสถานะ
        transform.position = startPos;
        gameObject.SetActive(true);
        isDisintegrating = false;
        isFloating = true;  // เริ่มการลอยใหม่
    }
}
