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
  if(blinkCooldownTimer > 0)
        {
            blinkCooldownTimer -= Time.deltaTime;

        }
      if (Input.GetKeyDown(KeyCode.C) && blinkCooldownTimer <= 0)
      {
        StartCoroutine(Blinkanime());


      }
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




    public void Blink()
    {
    
        float direction = transform.localScale.x < 0 ? 1 : -1;
        transform.position += new Vector3(blinkdistance * direction, 0, 0);
        blinkCooldownTimer = blinkCooldown;
       


    }

IEnumerator Blinkanime()
{
    anime.SetTrigger("blink");
    yield return new WaitForSeconds(0.5f);
    Blink();

}


    public void jumppp()
    {
         if(jumpCooldownTimer > 0)
        {
            jumpCooldownTimer -= Time.deltaTime;

        }

        if(Input.GetKeyDown(KeyCode.Space) && jumpCooldownTimer <= 0 && jumpCount < maxJump)

        {
             myRigid.velocity = Vector2.up * Jumpp;
             jumpCount = jumpCount + 1;
             jumpCooldownTimer = jumpCooldown;

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

