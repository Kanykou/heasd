using UnityEngine;

public class Player : MonoBehaviour
{
    public MonoBehaviour[] allSkills; // สคริปต์สกิลทั้งหมดที่อยู่ใน Player

    public void ApplySkillToPlayer(string skillName)
    {
        Debug.Log("Player acquired skill: " + skillName);

        // ปิดสกิลทั้งหมด
        foreach (MonoBehaviour skill in allSkills)
        {
            skill.enabled = false;
        }

        // เปิดสกิลที่ตรงกับชื่อที่เลือก
        switch (skillName)
        {
            case "Skill1":
                allSkills[0].enabled = true;
                break;
            case "Skill2":
                allSkills[1].enabled = true;
                break;
            case "Skill3":
                allSkills[2].enabled = true;
                break;
            default:
                Debug.LogWarning("Skill not found: " + skillName);
                break;
        }
    }
}
