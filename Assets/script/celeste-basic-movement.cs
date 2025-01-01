using UnityEngine;

public class CelesteBasicMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public Animator animator;

    [Header("Movement")]
    public float moveSpeed = 10f;
    public float acceleration = 50f;
    public float deceleration = 50f;
    private float horizontalInput;
    private float currentSpeed;

    [Header("Jump")]
    public float jumpForce = 15f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public int maxAirJumps = 1;
    private int airJumpCount;
      public float wallJumpControlLockTime = 0.2f; // เวลาที่จะล็อคการควบคุม
    private float controlLockTimer = 0f;
    private bool isControlLocked = false;
    
    [Header("Wall Movement")]
    public float wallSlideSpeed = 3f;
    public float wallJumpForce = 20f;
    public Vector2 wallJumpAngle = new Vector2(1, 2);
    public float wallCheckDistance = 0.4f;
    public LayerMask wallLayer;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private int facingDirection = 1;

    [Header("Effects")]
public ParticleSystem speedParticle; // ใส่ Particle สำหรับความเร็ว
public float speedThreshold = 15f;   // ความเร็วที่จะแสดง Particle

private void HandleParticleEffect()
{
    if (Mathf.Abs(rb.velocity.x) > speedThreshold)
    {
        if (!speedParticle.isPlaying)
        {
            speedParticle.Play(); // เล่น Particle เมื่อถึงความเร็วกำหนด
        }
    }
    else
    {
        if (speedParticle.isPlaying)
        {
            speedParticle.Stop(); // หยุด Particle เมื่อความเร็วต่ำกว่ากำหนด
        }
    }
}


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        wallJumpAngle.Normalize();
    }

   private void Update()
{
    UpdateTimers();
    CheckInput();
    CheckSurroundings();
    HandleMovement();
    HandleJump();
    HandleWallSlide();
    FlipCharacter();
    HandleParticleEffect();

    if (animator != null)
{
    animator.SetFloat("Speed", isGrounded ? Mathf.Abs(currentSpeed) : 0f);
    animator.SetBool("IsGrounded", isGrounded);
    animator.SetBool("IsWallSliding", isWallSliding);

    // ตั้งค่า Trigger สำหรับ Jump และ Fall
   
    if (!isGrounded && rb.velocity.y < 0)
    {
        animator.SetTrigger("Fall");
    }
}
}
private void UpdateTimers()
    {
        // อัพเดทเวลาล็อคการควบคุม
        if (isControlLocked)
        {
            controlLockTimer -= Time.deltaTime;
            if (controlLockTimer <= 0)
            {
                isControlLocked = false;
            }
        }
    }
    private void CheckInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded || (airJumpCount < maxAirJumps && !isWallSliding))
            {
                Jump();
                animator.SetTrigger("Jump");
            }
            else if (isWallSliding)
            {
                WallJump();
            }
        }
    }

    private void HandleMovement()
    {
        // ถ้าการควบคุมถูกล็อค ไม่รับ input
        if (isControlLocked)
        {
            return;
        }

        if (isWallSliding) return;

        float targetSpeed = horizontalInput * moveSpeed;
        currentSpeed = rb.velocity.x;
        float speedDiff = targetSpeed - currentSpeed;
        float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = speedDiff * accelerationRate * Time.deltaTime;
        
        rb.AddForce(movement * Vector2.right, ForceMode2D.Impulse);
        
        float maxSpeed = moveSpeed;
        rb.velocity = new Vector2(
            Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed),
            rb.velocity.y
        );
    }

    private void HandleJump()
    {
        if (rb.velocity.y < 0)
        {
            // เพิ่มแรงโน้มถ่วงขณะตกลงมา * Time.deltaTime
            rb.gravityScale = fallMultiplier * Time.deltaTime * 120;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            // เพิ่มแรงโน้มถ่วงเมื่อปล่อยปุ่มกระโดด * Time.deltaTime
            rb.gravityScale = lowJumpMultiplier * Time.deltaTime * 120;
        }
        else
        {
            // คืนค่าแรงโน้มถ่วงปกติ
            rb.gravityScale = 2f;
        }
        
    }

    private void Jump()
    {
        // กำหนดความเร็วในการกระโดดแบบทันที
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        if (!isGrounded) airJumpCount++;
    }

     private void WallJump()
    {
        animator.SetTrigger("");
        Vector2 jumpDirection = new Vector2(-facingDirection * wallJumpAngle.x, wallJumpAngle.y);
        rb.velocity = Vector2.zero;
        rb.AddForce(jumpDirection * wallJumpForce, ForceMode2D.Impulse);
        
        // เริ่มการล็อคควบคุม
        controlLockTimer = wallJumpControlLockTime;
        isControlLocked = true;
        
        airJumpCount = 0;
    }

    private void HandleWallSlide()
    {
        isWallSliding = isTouchingWall && !isGrounded && rb.velocity.y < 0;

        if (isWallSliding)
        {
            // ปรับความเร็วการเลื่อนลงกำแพง * Time.deltaTime
            float slideVelocity = -wallSlideSpeed * Time.deltaTime * 60;
            rb.velocity = new Vector2(rb.velocity.x, slideVelocity);
            airJumpCount = 0;
        }
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        
        Vector2 wallCheckDirection = Vector2.right * facingDirection;
        isTouchingWall = Physics2D.Raycast(transform.position, wallCheckDirection, wallCheckDistance, wallLayer);

        if (isGrounded)
        {
            airJumpCount = 0;
        }
    }

    private void FlipCharacter()
    {
        if (horizontalInput != 0)
        {
            facingDirection = (int)Mathf.Sign(horizontalInput);
            transform.localScale = new Vector3(facingDirection, 1, 1);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);

        Gizmos.color = isTouchingWall ? Color.blue : Color.yellow;
        Vector3 wallDirection = Vector3.right * facingDirection;
        Gizmos.DrawLine(transform.position, transform.position + wallDirection * wallCheckDistance);
    }
}