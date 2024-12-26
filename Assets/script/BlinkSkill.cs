using UnityEngine;

public class BlinkSkill : MonoBehaviour
{
    public float blinkDistance = 5f; // ระยะทางในการ Blink
    public LayerMask obstacleLayer; // ชั้นที่ตรวจสอบการชน

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Blink();
        }
    }

    private void Blink()
    {
        Vector3 blinkDirection = transform.localScale.x < 0 ? Vector3.right : Vector3.left; // ทิศทางการ Blink
        Vector3 targetPosition = transform.position + blinkDirection * blinkDistance;

        // ตรวจสอบว่ามีสิ่งกีดขวางหรือไม่
        RaycastHit2D hit = Physics2D.Raycast(transform.position, blinkDirection, blinkDistance, obstacleLayer);
        if (hit.collider != null)
        {
            targetPosition = hit.point; // ย้ายไปจนถึงจุดชน
        }

        transform.position = targetPosition; // เคลื่อนย้ายตัวละคร
        Debug.Log("Blinked to: " + targetPosition);
    }
}
