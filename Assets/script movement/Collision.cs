using UnityEngine;

public class Collision : MonoBehaviour
{
    [Header("Layers")]
    public LayerMask groundLayer;

    [Space]

    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public bool canLedgeClimb; // เพิ่มตัวแปรสำหรับเช็คการปีนขอบ
    public int wallSide;

    [Space]

    [Header("Collision")]
    public float collisionRadius = 0.25f;
    public Vector2 bottomOffset, rightOffset, leftOffset;
    
    [Header("Ledge Detection")]
    public Vector2 ledgeCheckOffset = new Vector2(0.5f, 1f); // ตำแหน่งที่จะเช็คขอบ
    private Color debugCollisionColor = Color.red;

    void Update()
    {  
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
        onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer) 
            || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        wallSide = onRightWall ? -1 : 1;

        // เช็คการปีนขอบ
        if (onWall && !onGround)
        {
            Vector2 ledgeCheckPosition = (Vector2)transform.position;
            // ปรับตำแหน่งตามด้านที่ชนกำแพง
            if (onRightWall)
            {
                ledgeCheckPosition += new Vector2(ledgeCheckOffset.x, ledgeCheckOffset.y);
            }
            else if (onLeftWall)
            {
                ledgeCheckPosition += new Vector2(-ledgeCheckOffset.x, ledgeCheckOffset.y);
            }

            // ถ้าไม่มีกำแพงที่ตำแหน่งเช็ค แสดงว่าสามารถปีนขึ้นได้
            canLedgeClimb = !Physics2D.OverlapCircle(ledgeCheckPosition, collisionRadius, groundLayer);
        }
        else
        {
            canLedgeClimb = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);

        // วาด Gizmo สำหรับจุดเช็คขอบ
        if (onWall && !onGround)
        {
            Gizmos.color = canLedgeClimb ? Color.green : Color.red;
            Vector2 ledgeCheckPosition = (Vector2)transform.position;
            if (onRightWall)
            {
                ledgeCheckPosition += new Vector2(ledgeCheckOffset.x, ledgeCheckOffset.y);
            }
            else if (onLeftWall)
            {
                ledgeCheckPosition += new Vector2(-ledgeCheckOffset.x, ledgeCheckOffset.y);
            }
            Gizmos.DrawWireSphere(ledgeCheckPosition, collisionRadius);
        }
    }
}