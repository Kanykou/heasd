using UnityEngine;

public class WallSlideAndJumpSkill : MonoBehaviour
{
    public float wallSlideSpeed = 2f; // ความเร็วการไถล
    public float wallClimbSpeed = 3f; // ความเร็วในการปีนกำแพง
    public float wallJumpForce = 10f; // แรงกระโดดจากกำแพง
    public Vector2 wallJumpDirection = new Vector2(1, 1); // ทิศทางการกระโดดจากกำแพง
    public LayerMask wallLayer; // เลเยอร์ของกำแพง
    public float wallGrabDuration = 5f; // ระยะเวลาที่สามารถเกาะกำแพงได้

public float rbgravityOriginal = 2f;
    private Rigidbody2D rb;
    private bool isWallGrabbing = false;
    private bool isWallSliding = false;
    private float wallGrabTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        wallJumpDirection.Normalize(); // ทำให้ทิศทางเป็นหน่วย (1)
    }

    void Update()
    {
        // ตรวจสอบว่าตัวละครสัมผัสกำแพงหรือไม่
        bool isTouchingWall = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, 0.5f, wallLayer);

        // กด K เพื่อเกาะกำแพง
        if (Input.GetKey(KeyCode.K) && isTouchingWall)
        {
            StartWallGrab();
        }
        else
        {
            StopWallGrab();
        }

        // หากเกาะกำแพง ให้จับเวลา
        if (isWallGrabbing)
        {
            wallGrabTimer += Time.deltaTime;
            if (wallGrabTimer >= wallGrabDuration) // ปล่อยกำแพงเมื่อถึงเวลา
            {
                StopWallGrab();
            }

            // ปีนกำแพงขึ้น-ลง
            float verticalInput = Input.GetAxisRaw("Vertical");
            rb.velocity = new Vector2(0, verticalInput * wallClimbSpeed);
        }

        // หากกำลังไถลกำแพง
        if (isWallSliding && rb.velocity.y < 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
        }

        // กระโดดออกจากกำแพง
        if (isWallGrabbing && Input.GetKeyDown(KeyCode.Space))
        {
            WallJump();
        }
    }

    void StartWallGrab()
    {
        isWallGrabbing = true;
        isWallSliding = false;
        rb.gravityScale = 0; // ปิดแรงโน้มถ่วงชั่วคราว
        rb.velocity = Vector2.zero; // หยุดการเคลื่อนที่
        wallGrabTimer = 0f; // รีเซ็ตตัวจับเวลา
    }

    void StopWallGrab()
    {
        isWallGrabbing = false;
        isWallSliding = true;
        rb.gravityScale = rbgravityOriginal; // คืนค่าแรงโน้มถ่วง
    }

    void WallJump()
    {
        StopWallGrab(); // หยุดเกาะกำแพง
        rb.velocity = Vector2.zero; // รีเซ็ตความเร็ว
        rb.AddForce(new Vector2(-transform.localScale.x * wallJumpDirection.x, wallJumpDirection.y) * wallJumpForce, ForceMode2D.Impulse);
    }

    private void OnDrawGizmos()
    {
        // แสดง Raycast สำหรับตรวจจับกำแพงใน Editor
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.right * transform.localScale.x * 0.5f);
    }
}
