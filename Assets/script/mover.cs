using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class mover : MonoBehaviour
{
    private float moveH;
    Rigidbody2D myRigid;

    public float speed = 3f;
    public float Jumpp = 2f;
   public float jumpCooldown = 1f;
     private float jumpCooldownTimer = 0f;
     public float shiftSpeed = 5f;
     public float blinkdistance = 1f;
     public float blinkCooldown = 1f;
     private float blinkCooldownTimer = 0f;
     private int jumpCount = 0;
     public int maxJump = 2;
     public Animator anime;






    // Start is called before the first frame update
    void Start()
    {
         myRigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
      movee();
    }



    public void movee()
    {
        jumppp();
        float CurrentSpeed = speed;
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) 
        {
            CurrentSpeed = shiftSpeed;
        }
        moveH = Input.GetAxis("Horizontal")  * CurrentSpeed ;
        myRigid.velocity = new Vector2(moveH, myRigid.velocity.y);
        anime.SetFloat("Speed", Mathf.Abs(moveH));
        

        if(moveH < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (moveH > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    public void jumppp()
{
    // ลดตัวจับเวลาคูลดาวน์
    if (jumpCooldownTimer > 0)
    {
        jumpCooldownTimer -= Time.deltaTime;
    }

    // กระโดดเมื่อกดปุ่ม Space และยังมีจำนวนกระโดดเหลืออยู่
    if (Input.GetKeyDown(KeyCode.Space) && jumpCooldownTimer <= 0 && jumpCount < maxJump)
    {
        myRigid.velocity = new Vector2(myRigid.velocity.x, Jumpp); // กระโดด
        jumpCount++; // เพิ่มจำนวนครั้งที่กระโดด
        jumpCooldownTimer = jumpCooldown; // ตั้งคูลดาวน์ใหม่
    }

    // ปรับฟิสิกส์ขณะตกหรือปล่อยปุ่มกระโดด
    if (myRigid.velocity.y < 0)
    {
        // ตกเร็วขึ้นด้วย Fall Multiplier
        myRigid.velocity += Vector2.up * Physics2D.gravity.y * (2.5f - 1) * Time.deltaTime;
    }
    else if (myRigid.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
    {
        // ตกเร็วขึ้นเล็กน้อยเมื่อปล่อยปุ่มกระโดด
        myRigid.velocity += Vector2.up * Physics2D.gravity.y * (2f - 1) * Time.deltaTime;
    }
}
    public void OnCollisionEnter2D(Collision2D collisionInfo)

    {
        if(collisionInfo.gameObject.tag==("ground"))
        {
            jumpCount = 0;

        }

    }
}

