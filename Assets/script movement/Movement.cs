using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Movement : MonoBehaviour
{
    private Collision coll;
    [HideInInspector]
    public Rigidbody2D rb;
    private AnimationScript anim;

    [Space]
    [Header("Stats")]
    public float speed = 10;
    public float jumpForce = 50;
    public float slideSpeed = 5;
    public float wallJumpLerp = 10;
    public float dashSpeed = 20;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool wallGrab;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;

    [Space]
    [Header("Facing Direction")]
    [SerializeField] private bool autoFlip = true; // ควบคุมว่าจะให้หันหน้าอัตโนมัติตามการเคลื่อนที่หรือไม่
    [SerializeField] private float flipThreshold = 0.1f; // ความเร็วขั้นต่ำในการหันหน้า

    [Space]
    private bool groundTouch;
    private bool hasDashed;
    public int side = 1;
    private float lastMoveDirection;

     [Space]
    [Header("Ledge Climb")]
    [SerializeField] private Vector2 ledgeClimbOffset = new Vector2(0.5f, 1.5f);
    [SerializeField] private float ledgeClimbDuration = 0.3f;
    [SerializeField] private float autoClimbDelay = 0.1f; // ดีเลย์เล็กน้อยก่อนปีนอัตโนมัติ
    private bool isLedgeClimbing;
    private bool wasHoldingUp;

    [Space]
    [Header("Polish")]
    public ParticleSystem dashParticle;
    public ParticleSystem jumpParticle;
    public ParticleSystem wallJumpParticle;
    public ParticleSystem slideParticle;

    private RaycastHit2D ledgeRaycast;


    void Start()
    {
    
        canMove = true;
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<AnimationScript>();
        lastMoveDirection = side;
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        Walk(dir);
        anim.SetHorizontalMovement(x, y, rb.velocity.y);

        // ระบบหันหน้า
        HandleFacing(x);

        if (coll.onWall && Input.GetButton("Fire3") && canMove)
        {
            if(side != coll.wallSide)
                anim.Flip(side*-1);
            wallGrab = true;
            wallSlide = false;
        }

        if (Input.GetButtonUp("Fire3") || !coll.onWall || !canMove)
        {
            wallGrab = false;
            wallSlide = false;
        }

        if (coll.onGround && !isDashing)
        {
            wallJumped = false;
            GetComponent<BetterJumping>().enabled = true;
        }
        
        if (wallGrab && !isDashing)
        {
            rb.gravityScale = 0;
            if(x > .2f || x < -.2f)
                rb.velocity = new Vector2(rb.velocity.x, 0);

            float speedModifier = y > 0 ? .5f : 1;
            rb.velocity = new Vector2(rb.velocity.x, y * (speed * speedModifier));
        }
        else
        {
            rb.gravityScale = 3;
        }

        if(coll.onWall && !coll.onGround)
        {
            if (x != 0 && !wallGrab)
            {
                wallSlide = true;
                WallSlide();
            }
        }

        if (!coll.onWall || coll.onGround)
            wallSlide = false;

        if (Input.GetButtonDown("Jump"))
        {
            anim.SetTrigger("jump");

            if (coll.onGround)
                Jump(Vector2.up, false);
            if (coll.onWall && !coll.onGround)
                WallJump();
        }

          bool isHoldingUp = Input.GetAxis("Vertical") > 0;
        bool justPressedUp = Input.GetButtonDown("Vertical") && isHoldingUp;

        if (coll.canLedgeClimb && !isLedgeClimbing)
        {
            if (justPressedUp || (isHoldingUp && !wasHoldingUp))
            {
                // กดปุ่มขึ้นใหม่
                StartCoroutine(LedgeClimbCoroutine(0f));
            }
            else if (isHoldingUp && wasHoldingUp)
            {
                // กดค้างมาตั้งแต่ก่อนถึงขอบ
                StartCoroutine(LedgeClimbCoroutine(autoClimbDelay));
            }
        }

        wasHoldingUp = isHoldingUp;
        
    }


    // ฟังก์ชั่นใหม่สำหรับจัดการการหันหน้า
    private void HandleFacing(float horizontalInput)
    {
        if (!autoFlip || wallGrab || wallSlide || !canMove) return;

        // หันหน้าตามการเคลื่อนที่
        if (Mathf.Abs(horizontalInput) > flipThreshold)
        {
            int newSide = horizontalInput > 0 ? 1 : -1;
            if (newSide != side)
            {
                side = newSide;
                anim.Flip(side);
                lastMoveDirection = horizontalInput;
            }
        }
    }

     
    private IEnumerator LedgeClimbCoroutine(float initialDelay)
    {
        if (isLedgeClimbing) yield break;

        if (initialDelay > 0)
        {
            yield return new WaitForSeconds(initialDelay);
            // เช็คอีกครั้งหลังดีเลย์ว่ายังสามารถปีนได้อยู่หรือไม่
            if (!coll.canLedgeClimb || Input.GetAxis("Vertical") == 0) 
            {
                yield break;
            }
        }

        isLedgeClimbing = true;
        canMove = false;
        wallGrab = false;
        wallSlide = false;

        // เก็บตำแหน่งเริ่มต้น
        Vector2 startPos = transform.position;
        
        // คำนวณตำแหน่งสุดท้าย
        Vector2 endPos = transform.position;
        endPos += new Vector2(ledgeClimbOffset.x * (coll.onRightWall ? 1 : -1), ledgeClimbOffset.y);

        // ปิดการทำงานของ Rigidbody ชั่วคราว
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;

        // เคลื่อนที่ไปยังตำแหน่งสุดท้าย
        float elapsedTime = 0;
        while (elapsedTime < ledgeClimbDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / ledgeClimbDuration;
            t = t * t * (3f - 2f * t); // smoothstep
            transform.position = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        // คืนค่าการทำงานต่างๆ
        rb.gravityScale = 3;
        canMove = true;
        isLedgeClimbing = false;
    }

    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;

        side = anim.sr.flipX ? -1 : 1;
        jumpParticle.Play();
    }

    
    private void WallJump()
    {
        // หันหน้าทันทีตามทิศทางที่จะกระโดด
        side = coll.onRightWall ? -1 : 1;
        anim.Flip(side);

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;
        Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);

        wallJumped = true;
        StartCoroutine(AutoFlipAfterWallJump());
    }

    private IEnumerator AutoFlipAfterWallJump()
    {
        // รอให้การกระโดดเริ่มต้นก่อน
        yield return new WaitForSeconds(0.2f);

        while (wallJumped && !coll.onGround && !coll.onWall)
        {
            float xInput = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(xInput) > 0.1f)
            {
                side = (xInput > 0) ? 1 : -1;
                anim.Flip(side);
                break;
            }
            yield return null;
        }
    }
    private void WallSlide()
    {
        // ตรวจสอบและหันหน้าเข้าหากำแพงเสมอ
        int wallSide = coll.onRightWall ? 1 : -1;
        if(side != wallSide)
        {
            side = wallSide;
            anim.Flip(side);
        }

        if (!canMove)
            return;

        bool pushingWall = false;
        if((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(push, -slideSpeed);
    }

    private void Walk(Vector2 dir)
    {
        if (!canMove)
            return;

        if (wallGrab)
            return;

        if (!wallJumped)
        {
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
        }
    }

    private void Jump(Vector2 dir, bool wall)
    {
        slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
        ParticleSystem particle = wall ? wallJumpParticle : jumpParticle;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;

        particle.Play();
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }

    void WallParticle(float vertical)
    {
        var main = slideParticle.main;

        if (wallSlide || (wallGrab && vertical < 0))
        {
            slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
            main.startColor = Color.white;
        }
        else
        {
            main.startColor = Color.clear;
        }
    }

    int ParticleSide()
    {
        int particleSide = coll.onRightWall ? 1 : -1;
        return particleSide;
    }
}