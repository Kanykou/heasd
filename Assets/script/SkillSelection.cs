using System.Collections.Generic;
using UnityEngine;

public class SkillSelection : MonoBehaviour
{
    public GameObject skillWheelUI; // UI ของวงล้อเลือกสกิล
    public AudioSource backgroundMusic; // เพลงพื้นหลัง
    public SkillWheelController skillWheelController; // ตัวควบคุมวงล้อ
    public float slowMotionTarget = 0.2f; // เป้าหมายความเร็วเวลาที่ช้าลง
    public float timeLerpSpeed = 2f; // ความเร็วในการเปลี่ยน Time.timeScale
    public float musicPitchSlow = 0.5f; // Pitch เพลงตอน Slow
    private float originalTimeScale = 1f; // ค่า TimeScale ปกติ
    private float originalPitch = 1f; // ค่า Pitch ปกติ
    private bool isSelectingSkill = false; // สถานะกำลังเลือกสกิล
    private bool isResettingTime = false; // สถานะกำลังคืนค่าเวลา
    public Dictionary<string, MonoBehaviour> skillMap; // Mapping ระหว่างชื่อสกิลและสคริปต์
    public MonoBehaviour playerMoverScript; // สคริปต์ควบคุมการเคลื่อนที่ของ Player

    void Start()
{
    // กำหนดค่าเริ่มต้นให้ SkillMap
    skillMap = new Dictionary<string, MonoBehaviour>();

    // ดึง Component สกิล
    MonoBehaviour dashSkill = GetComponent<DashSkill>();
    MonoBehaviour blinkSkill = GetComponent<BlinkSkill>();
    MonoBehaviour wallSkill = GetComponent<WallSlideAndJumpSkill>();

    // เพิ่ม Component ลงใน SkillMap ถ้ามีอยู่
    if (dashSkill != null)
    {
        skillMap.Add("Dash", dashSkill);
        Debug.Log("DashSkill added to SkillMap.");
    }
    else
    {
        Debug.LogWarning("DashSkill component is missing!");
    }

    if (blinkSkill != null)
    {
        skillMap.Add("Blink", blinkSkill);
        Debug.Log("BlinkSkill added to SkillMap.");
    }
    else
    {
        Debug.LogWarning("BlinkSkill component is missing!");
    }

    if (wallSkill != null)
    {
        skillMap.Add("WallSlideAndJumpSkill", wallSkill);
        Debug.Log("WallSlideAndJumpSkill added to SkillMap.");
    }
    else
    {
        Debug.LogWarning("WallSlideAndJumpSkill component is missing!");
    }

    // ตรวจสอบว่า SkillMap มีสกิลอย่างน้อย 1 สกิล
    if (skillMap.Count == 0)
    {
        Debug.LogError("SkillMap is empty. Please add skill components to the GameObject.");
    }
}

    void Update()
    {
        // กดค้าง R เพื่อเข้าสู่โหมด Slow Motion
        if (Input.GetKey(KeyCode.R))
        {
            isResettingTime = false; // หยุดคืนค่าเวลา
            SlowMotion(); // เรียกใช้ฟังก์ชัน Slow Motion
            if (!isSelectingSkill && Time.unscaledTime >= slowMotionTarget) // ใช้เวลาแบบไม่หน่วง
            {
                ActivateSkillSelection();
            }
        }

        // ปล่อย R เพื่อยืนยันการเลือก
        if (Input.GetKeyUp(KeyCode.R))
        {
            ConfirmSkillSelection(); // ยืนยันสกิลที่เลือกจาก Skill Wheel
            isResettingTime = true; // เริ่มคืนค่าเวลา
        }

        // คืนค่าเวลาเมื่อกำลัง Reset
        if (isResettingTime)
        {
            ResetTime();
        }
    }

    // ฟังก์ชัน Slow Motion (ลดเวลาแบบไม่ขึ้นกับ Time.timeScale)
    void SlowMotion()
    {
        Time.timeScale = Mathf.Lerp(Time.timeScale, slowMotionTarget, timeLerpSpeed * Time.unscaledDeltaTime);
        backgroundMusic.pitch = Mathf.Lerp(backgroundMusic.pitch, musicPitchSlow, timeLerpSpeed * Time.unscaledDeltaTime);
    }

    // ฟังก์ชัน Reset Time (คืนค่าเวลาแบบไม่ขึ้นกับ Time.timeScale)
    void ResetTime()
    {
        Time.timeScale = Mathf.MoveTowards(Time.timeScale, originalTimeScale, timeLerpSpeed * Time.unscaledDeltaTime);
        backgroundMusic.pitch = Mathf.MoveTowards(backgroundMusic.pitch, originalPitch, timeLerpSpeed * Time.unscaledDeltaTime);

        if (Mathf.Abs(Time.timeScale - originalTimeScale) < 0.01f && Mathf.Abs(backgroundMusic.pitch - originalPitch) < 0.01f)
        {
            Time.timeScale = originalTimeScale;
            backgroundMusic.pitch = originalPitch;
            isResettingTime = false;
        }
    }

    void ActivateSkillSelection()
    {
        if (skillWheelUI == null || skillWheelController == null || playerMoverScript == null)
        {
            Debug.LogError("ActivateSkillSelection: Missing references. Please check skillWheelUI, skillWheelController, and playerMoverScript.");
            return;
        }

        isSelectingSkill = true;
        skillWheelUI.SetActive(true);
        skillWheelController.ResetSkillSelection();
        Cursor.visible = true;
        playerMoverScript.enabled = false;
    }

    void ConfirmSkillSelection()
    {
        Debug.Log("Skill Selected: " + skillWheelController.GetCurrentSkillName());

        // ส่งผลให้ Player ได้รับสกิล
        ApplySkillToPlayer(skillWheelController.GetCurrentSkillName());

        // ปิดโหมดเลือกสกิล
        skillWheelUI.SetActive(false);
        isSelectingSkill = false;
        playerMoverScript.enabled = true;
    }

    public void ApplySkillToPlayer(string skillName)
    {
        if (skillMap == null || skillMap.Count == 0)
        {
            Debug.LogError("SkillMap is null or empty.");
            return;
        }

        foreach (var skill in skillMap.Values)
        {
            if (skill != null)
            {
                skill.enabled = false; // ปิดทุกสกิล
            }
            else
            {
                Debug.LogWarning("Skill in SkillMap is null. Please check the initialization.");
            }
        }

        if (skillMap.ContainsKey(skillName))
        {
            skillMap[skillName].enabled = true; // เปิดสกิลที่เลือก
            Debug.Log($"Activated Skill: {skillName}");
        }
        else
        {
            Debug.LogWarning($"Skill not found: {skillName}");
        }
    }
}
