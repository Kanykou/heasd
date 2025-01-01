using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    private float moveH;
    private Rigidbody2D myRigid;

    public float speed = 3f;
    public float jumpForce = 2f;
    public int maxJump = 2; // จำนวนครั้งที่กระโดดได้ (เช่น Double Jump)
    public float wallJumpForce = 5f; // แรงกระโดดออกจากกำแพง
    public Vector2 wallJumpDirection = new Vector2(1, 1); // ทิศทางการกระโดดจากกำแพง
    public LayerMask groundLayer; // เลเยอร์ของพื้น
    public LayerMask wallLayer; // เลเยอร์ของกำแพง
    private int jumpCount = 0;
    private bool isGrounded = false;
    private bool isTouchingWall = false;
    public float wallCheckDistance = 0.5f; // ระยะตรวจจับกำแพง
    public Animator anime;

    void Start()
    {
        myRigid = GetComponent<Rigidbody2D>();
        wallJumpDirection.Normalize(); // ทำให้ทิศทางการกระโดดเป็นหน่วย (1)
    }

    void Update()
    {
        movee();
    }

    public void movee()
    {
        jumppp();

        // การเคลื่อนที่แนวนอน
        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed *= 1.5f; // เพิ่มความเร็วเมื่อกด Shift
        }

        moveH = Input.GetAxis("Horizontal") * currentSpeed;
        myRigid.velocity = new Vector2(moveH, myRigid.velocity.y);
        anime.SetFloat("Speed", Mathf.Abs(moveH));

        // ปรับทิศทางตัวละคร
        if (moveH < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (moveH > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    public void jumppp()
    {
        // ตรวจสอบสถานะสัมผัสพื้นและกำแพง
        isGrounded = CheckGround();
        isTouchingWall = CheckWall();

        // รีเซ็ตการกระโดดเมื่อสัมผัสพื้น
        if (isGrounded)
        {
            Debug.Log("โดนพื้นแล้ว");
            jumpCount = 0;
        }

        // กระโดดจากกำแพง
        if (isTouchingWall && Input.GetKeyDown(KeyCode.Space) && !isGrounded)
        {
            WallJump();
        }
        // กระโดดปกติ
        else if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJump)
        {
            myRigid.velocity = new Vector2(myRigid.velocity.x, jumpForce);
            jumpCount++;
        }
    }

    private void WallJump()
    {
        Vector2 jumpDir = new Vector2(-transform.localScale.x * wallJumpDirection.x, wallJumpDirection.y);
        myRigid.velocity = Vector2.zero; // รีเซ็ตความเร็วปัจจุบัน
        myRigid.AddForce(jumpDir * wallJumpForce, ForceMode2D.Impulse);
        jumpCount++; // เพิ่มจำนวนกระโดด
    }

    private bool CheckGround()
    {
        // ใช้ Raycast ตรวจสอบว่าตัวละครสัมผัสพื้นหรือไม่
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2f, groundLayer);
        Debug.DrawRay(transform.position, Vector2.down * 0.1f, Color.green);
        return hit.collider != null;
    }

    private bool CheckWall()
    {
        // ใช้ Raycast ตรวจสอบว่าตัวละครสัมผัสกำแพงหรือไม่
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, wallCheckDistance, wallLayer);
        Debug.DrawRay(transform.position, direction * wallCheckDistance, Color.red);
        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        // วาด Raycast สำหรับตรวจพื้น
        Gizmos.color = isGrounded ? Color.green : Color.red; // สีเขียวเมื่อสัมผัสพื้น, สีแดงเมื่อไม่สัมผัส
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 0.1f);

        // วาด Raycast สำหรับตรวจจับกำแพง
        Gizmos.color = isTouchingWall ? Color.blue : Color.yellow; // สีน้ำเงินเมื่อสัมผัสกำแพง, สีเหลืองเมื่อไม่สัมผัส
        Vector3 wallDirection = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
        Gizmos.DrawLine(transform.position, transform.position + wallDirection * wallCheckDistance);
    }
}
