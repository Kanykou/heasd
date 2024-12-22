using UnityEngine;

public class WallSlideAndJumpSkill : MonoBehaviour
{
    public float wallSlideSpeed = 2f; // ความเร็วการไถล
    public float wallJumpForce = 10f; // แรงกระโดดจากกำแพง
    public Vector2 wallJumpDirection = new Vector2(1, 1); // ทิศทางการกระโดดจากกำแพง
    public LayerMask wallLayer; // เลเยอร์ของกำแพง

    private Rigidbody2D rb;
    private bool isWallSliding = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        wallJumpDirection.Normalize(); // ทำให้ทิศทางเป็นหน่วย (1)
    }

    void Update()
    {
        bool isTouchingWall = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, 0.5f, wallLayer);

        if (isTouchingWall && rb.velocity.y < 0) // ตรวจสอบการสัมผัสกำแพง
        {
            StartWallSlide();
        }
        else
        {
            StopWallSlide();
        }

        if (isWallSliding && Input.GetKeyDown(KeyCode.Space)) // กระโดดออกจากกำแพง
        {
            WallJump();
        }
    }

    void StartWallSlide()
    {
        isWallSliding = true;
        rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed); // ลดความเร็วแกน Y
    }

    void StopWallSlide()
    {
        isWallSliding = false;
    }

    void WallJump()
    {
        rb.velocity = new Vector2(0, 0); // รีเซ็ตความเร็ว
        rb.AddForce(new Vector2(-transform.localScale.x * wallJumpDirection.x, wallJumpDirection.y) * wallJumpForce, ForceMode2D.Impulse);
    }
}
