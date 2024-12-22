using UnityEngine;

public class SkillCreatePlatform : MonoBehaviour
{
    public GameObject platformPrefab; // Prefab ของ Platform
    public float platformLifetime = 5f; // ระยะเวลาที่ Platform จะอยู่
    public float platformDistance = 1f; // ระยะห่างของ Platform จากตัวละคร
    public float platformHeightOffset = -0.5f; // ความสูงของ Platform เมื่อสร้าง
    public float platformAngle = 20f; // มุมเฉียงของ Platform
    public KeyCode createPlatformKey = KeyCode.F; // ปุ่มสำหรับใช้สร้าง Platform

    private void Update()
    {
        if (Input.GetKeyDown(createPlatformKey))
        {
            CreatePlatform(); // สร้าง Platform เมื่อกดปุ่ม
        }
    }

    private void CreatePlatform()
    {
        // กำหนดทิศทางตามตัวละครหันหน้า (ซ้าย/ขวา)
        float direction = transform.localScale.x < 0 ? 1 : -1;

        // คำนวณตำแหน่งของ Platform
        Vector3 platformPosition = transform.position + new Vector3(platformDistance * direction, platformHeightOffset, 0);

        // สร้าง Platform
        GameObject newPlatform = Instantiate(platformPrefab, platformPosition, Quaternion.identity);

        // หมุน Platform ตามมุมเฉียงที่กำหนด
        newPlatform.transform.rotation = Quaternion.Euler(0, 0, platformAngle * direction);

        // ทำลาย Platform หลังจากเวลาที่กำหนด
        Destroy(newPlatform, platformLifetime);
    }
}
