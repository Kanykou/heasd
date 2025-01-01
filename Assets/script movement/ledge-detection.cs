using UnityEngine;

public class LedgeDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private Vector2 detectorSize = new Vector2(0.3f, 0.3f);
    [SerializeField] private Vector2 detectorOffset = new Vector2(0.5f, 1f);
    
    private Movement movement;
    private bool canClimb;
    
    private void Start()
    {
        movement = GetComponentInParent<Movement>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // เช็คว่าเป็น ground layer หรือไม่
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            canClimb = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // เมื่อไม่มีอะไรอยู่ในพื้นที่เช็ค
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            canClimb = true;
        }
    }

    public bool CanClimbLedge()
    {
        return canClimb;
    }

    private void OnDrawGizmos()
    {
        // แสดง Gizmo สำหรับดูพื้นที่การเช็ค
        Gizmos.color = Color.yellow;
        Vector3 position = transform.position + new Vector3(detectorOffset.x, detectorOffset.y, 0);
        Gizmos.DrawWireCube(position, new Vector3(detectorSize.x, detectorSize.y, 0));
    }
}
