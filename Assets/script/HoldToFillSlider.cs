using UnityEngine;
using UnityEngine.UI;

public class HoldToFillSlider : MonoBehaviour
{
    public Slider holdSlider; // Slider UI ที่จะแสดง
    public float holdTime = 1f; // เวลากดค้างให้เต็ม (1 วินาที)
    private float holdTimer = 0f; // ตัวจับเวลาการกดค้าง
    private bool isComplete = false; // สถานะเมื่อหลอดเต็ม

    void Start()
    {
        if (holdSlider != null)
        {
            holdSlider.maxValue = holdTime; // กำหนดค่าเต็มของ Slider
            holdSlider.value = 0f; // เริ่มต้นที่ 0
        }
    }

    void Update()
    {
        if (isComplete) return; // หากหลอดเต็มแล้ว ไม่ต้องทำอะไรต่อ

        if (Input.GetKey(KeyCode.R)) // กดปุ่ม R ค้าง
        {
            holdTimer += Time.unscaledDeltaTime; // ใช้ Time.unscaledDeltaTime เพื่อไม่ให้ Time.timeScale มีผล
            if (holdSlider != null)
            {
                holdSlider.value = holdTimer; // อัปเดตค่า Slider
            }

            if (holdTimer >= holdTime) // กดค้างครบตามเวลาที่กำหนด
            {
                CompleteFill();
            }
        }
    }

    void CompleteFill()
    {
        isComplete = true; // หลอดเต็มแล้ว
        holdTimer = holdTime; // ล็อคค่าเวลาไว้ที่เต็ม
        if (holdSlider != null)
        {
            holdSlider.value = holdTime; // ล็อค Slider ไว้ที่ค่าเต็ม
        }

        Debug.Log("Hold complete!"); // หรือเพิ่มการเรียกใช้งานฟังก์ชันอื่นที่ต้องการ
    }
}
