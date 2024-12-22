using UnityEngine;
using UnityEngine.UI;

public class SkillWheelController : MonoBehaviour
{
    public Image[] skillIcons; // ไอคอนของแต่ละสกิล
    public Text skillNameText; // ชื่อสกิลที่เลือก
    public string[] skillNames; // ชื่อของสกิลแต่ละอัน
    public MonoBehaviour[] skillScripts; // สคริปต์ของสกิลทั้งหมดใน Player
    public float radius = 100f; // รัศมีของวงล้อ

    private int currentSkillIndex = 0; // ตำแหน่งปัจจุบันของสกิลที่เลือก
    private int previousSkillIndex = -1; // เก็บตำแหน่งก่อนหน้าเพื่อลดการเปลี่ยนสถานะซ้ำ

    void Start()
    {
        PositionIcons(); // จัดตำแหน่งไอคอนรอบวงล้อ
        UpdateSkillHighlight(); // อัปเดตการไฮไลต์เริ่มต้น
    }

    void Update()
    {
        if (gameObject.activeSelf) // วงล้อแสดงอยู่
        {
            NavigateSkillWheel();

            // ยืนยันการเลือกเมื่อกด Enter
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ActivateSkillScript(currentSkillIndex);
            }
        }
    }

    public void ResetSkillSelection()
    {
        currentSkillIndex = 0; // รีเซ็ตตำแหน่งเลือกสกิล
        previousSkillIndex = -1; // รีเซ็ตตำแหน่งก่อนหน้า
        UpdateSkillHighlight();
    }

    void PositionIcons()
    {
        int totalSkills = skillIcons.Length;
        float angleStep = 360f / totalSkills; // มุมที่ไอคอนแต่ละอันจะห่างกัน

        for (int i = 0; i < totalSkills; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;

            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            skillIcons[i].rectTransform.anchoredPosition = new Vector2(x, y);
        }
    }

    void NavigateSkillWheel()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentSkillIndex = (currentSkillIndex + 1) % skillIcons.Length;
            UpdateSkillHighlight();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentSkillIndex = (currentSkillIndex - 1 + skillIcons.Length) % skillIcons.Length;
            UpdateSkillHighlight();
        }
    }

    void UpdateSkillHighlight()
    {
        if (previousSkillIndex != currentSkillIndex) // อัปเดตเฉพาะเมื่อมีการเปลี่ยนแปลง
        {
            if (previousSkillIndex >= 0 && previousSkillIndex < skillIcons.Length)
            {
                skillIcons[previousSkillIndex].color = Color.white; // คืนสีปกติ
            }

            skillIcons[currentSkillIndex].color = Color.yellow; // ไฮไลต์ไอคอนปัจจุบัน
            skillNameText.text = skillNames[currentSkillIndex]; // อัปเดตชื่อสกิล
            previousSkillIndex = currentSkillIndex; // เก็บตำแหน่งปัจจุบัน
        }
    }

    void ActivateSkillScript(int index)
    {
        if (index < 0 || index >= skillScripts.Length) return; // ตรวจสอบ Index ให้อยู่ในขอบเขต

        // ปิดสคริปต์ก่อนหน้า
        foreach (MonoBehaviour script in skillScripts)
        {
            if (script != null && script.enabled)
            {
                script.enabled = false;
            }
        }

        // เปิดสคริปต์ที่เลือก
        if (skillScripts[index] != null)
        {
            skillScripts[index].enabled = true;
            Debug.Log($"Activated Skill: {skillNames[index]}");
        }
    }

    public string GetCurrentSkillName()
    {
        return skillNames[currentSkillIndex]; // คืนค่าชื่อของสกิลที่เลือก
    }
}
