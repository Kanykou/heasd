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
     public Dictionary<string, MonoBehaviour> skillMap;


    public MonoBehaviour playerMoverScript; // สคริปต์ควบคุมการเคลื่อนที่ของ Player

    void Update()
    {
        // กดค้าง R เพื่อเข้าสู่โหมด Slow Motion
        if (Input.GetKey(KeyCode.R))
        {
            isResettingTime = false; // หยุดคืนค่าเวลา
            SlowMotion(); // เรียกใช้ฟังก์ชัน Slow Motion
            if (!isSelectingSkill && Time.timeScale <= slowMotionTarget + 0.05f) // เมื่อเวลาช้าพอแล้ว
            {
                ActivateSkillSelection();
            }
        }

        // ปล่อย R เพื่อเริ่มคืนค่าเวลา
        if (Input.GetKeyUp(KeyCode.R))
        {
            ConfirmSkillSelection(); // ยืนยันสกิลที่เลือกจาก Skill Wheel
            isResettingTime = true; // เริ่มคืนค่าเวลา
        }

        // คืนค่าเวลาเมื่อกำลัง Reset
        if (isResettingTime)
        {
            ResetTime(); // เรียกฟังก์ชันคืนเวลาใน Update
        }
    }

    // ฟังก์ชัน Slow Motion
    void SlowMotion()
    {
        Time.timeScale = Mathf.Lerp(Time.timeScale, slowMotionTarget, timeLerpSpeed * Time.unscaledDeltaTime); // ลด Time Scale
        backgroundMusic.pitch = Mathf.Lerp(backgroundMusic.pitch, musicPitchSlow, timeLerpSpeed * Time.unscaledDeltaTime); // ลด Pitch
    }

    // ฟังก์ชัน Reset Time
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
    void Start()
{
    if (skillMap == null || skillMap.Count == 0)
    {
        Debug.LogError("skillMap is null or empty. Please initialize it in Start.");

    }
    skillMap = new Dictionary<string, MonoBehaviour>
    {
        { "Dash", GetComponent<DashSkill>() },    // เชื่อม DashSkill
        { "Blink", GetComponent<BlinkSkill>() },  // เชื่อม BlinkSkill
        { "WallSlideAndJumpSkill", GetComponent<WallSlideAndJumpSkill>() }   // เชื่อม HoverSkill
    };
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