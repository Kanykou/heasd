using UnityEngine;

public class FloatingRock : MonoBehaviour
{
    public float amplitude = 0.5f; // ความสูงของการลอย
    public float frequency = 1f;   // ความเร็วของการลอย

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float yOffset = amplitude * Mathf.Sin(frequency * Time.time);
        transform.position = startPos + new Vector3(0, yOffset, 0);
    }
}
